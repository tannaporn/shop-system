using MediatR;
using Microsoft.EntityFrameworkCore;
using shop_api_layer.Contracts;
using shop_api_layer.Data;
using shop_api_layer.Enums;
using shop_api_layer.Exceptions;

namespace shop_api_layer.Features.Orders;

public sealed record CancelOrderCommand(Guid OrderId, Guid UserId) : IRequest;

public sealed class CancelOrderHandler(AppDbContext db) : IRequestHandler<CancelOrderCommand>
{
    public async Task Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        // shortcut: FOR UPDATE row-level lock is PostgreSQL-only; Serializable isolation gives equivalent protection
        await using var tx = await db.Database.BeginTransactionAsync(
            System.Data.IsolationLevel.Serializable, cancellationToken);

        var order = await db.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken)
            ?? throw new KeyNotFoundException($"Order {request.OrderId} not found.");

        var customer = await db.Customers.FirstOrDefaultAsync(c => c.UserId == request.UserId, cancellationToken)
            ?? throw new KeyNotFoundException("Customer not found.");

        if (order.CustomerId != customer.Id)
            throw new ForbiddenException("Order does not belong to this customer.");

        if (order.Status != OrderStatus.Pending)
            throw new ConflictException($"Cannot cancel order with status {order.Status}. Only Pending orders can be cancelled.");

        order.Status = OrderStatus.Cancelled;
        order.UpdatedAt = DateTime.UtcNow;

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

        await db.SaveChangesAsync(cancellationToken);
        await tx.CommitAsync(cancellationToken);
    }
}
