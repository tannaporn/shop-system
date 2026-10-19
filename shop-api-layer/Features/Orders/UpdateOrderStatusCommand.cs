using MediatR;
using Microsoft.EntityFrameworkCore;
using shop_api_layer.Data;
using shop_api_layer.Enums;
using shop_api_layer.Exceptions;

namespace shop_api_layer.Features.Orders;

public sealed record UpdateOrderStatusCommand(Guid OrderId, OrderStatus NewStatus) : IRequest;

public sealed class UpdateOrderStatusHandler(AppDbContext db) : IRequestHandler<UpdateOrderStatusCommand>
{
    private static readonly IReadOnlySet<(OrderStatus From, OrderStatus To)> AllowedTransitions =
        new HashSet<(OrderStatus, OrderStatus)>
        {
            (OrderStatus.Pending, OrderStatus.Confirmed),
            (OrderStatus.Confirmed, OrderStatus.Completed),
            (OrderStatus.Pending, OrderStatus.Cancelled),
            (OrderStatus.Confirmed, OrderStatus.Cancelled),
        };

    public async Task Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        await using var tx = await db.Database.BeginTransactionAsync(cancellationToken);

        var order = await db.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken)
            ?? throw new KeyNotFoundException($"Order {request.OrderId} not found.");

        if (!AllowedTransitions.Contains((order.Status, request.NewStatus)))
            throw new ConflictException(
                $"Cannot transition order from {order.Status} to {request.NewStatus}.");

        if (request.NewStatus == OrderStatus.Cancelled)
        {
            var productIds = order.Items.Select(i => i.ProductId).ToList();
            var products = await db.Products
                .Where(p => productIds.Contains(p.Id))
                .ToListAsync(cancellationToken);

            foreach (var item in order.Items)
            {
                var product = products.FirstOrDefault(p => p.Id == item.ProductId);
                if (product is not null)
                    product.StockQuantity += item.Quantity;
            }
        }

        order.Status = request.NewStatus;
        order.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(cancellationToken);
        await tx.CommitAsync(cancellationToken);
    }
}
