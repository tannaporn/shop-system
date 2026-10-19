using MediatR;
using Microsoft.EntityFrameworkCore;
using shop_api_layer.Data;
using shop_api_layer.Enums;
using shop_api_layer.Exceptions;

namespace shop_api_layer.Features.Products;

public sealed record DeleteProductCommand(Guid Id) : IRequest;

public sealed class DeleteProductHandler(AppDbContext db) : IRequestHandler<DeleteProductCommand>
{
    public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await db.Products
            .Where(p => p.Id == request.Id && p.DeletedAt == null)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new KeyNotFoundException($"Product {request.Id} was not found.");

        var hasActiveOrders = await db.OrderItems
            .Where(oi => oi.ProductId == request.Id)
            .Join(db.Orders,
                oi => oi.OrderId,
                o => o.Id,
                (oi, o) => o.Status)
            .AnyAsync(status => status == OrderStatus.Pending || status == OrderStatus.Confirmed,
                cancellationToken);

        if (hasActiveOrders)
            throw new ConflictException("Cannot delete product with pending or confirmed orders.");

        product.DeletedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(cancellationToken);
    }
}
