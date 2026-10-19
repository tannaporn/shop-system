using MediatR;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using shop_api_layer.Contracts.Orders;
using shop_api_layer.Data;
using shop_api_layer.Enums;
using shop_api_layer.Exceptions;
using shop_api_layer.Models.Order;

namespace shop_api_layer.Features.Orders;

public sealed record CheckoutCommand(Guid UserId) : IRequest<OrderResponse>;

public class CheckoutHandler(AppDbContext db) : IRequestHandler<CheckoutCommand, OrderResponse>
{
    // shortcut: lock strategy injected via virtual method so SQLite tests can skip FOR UPDATE
    protected virtual Task AcquireProductLocksAsync(
        IEnumerable<Guid> productIds, CancellationToken cancellationToken)
    {
        var ids = productIds.ToArray();
        var param = new NpgsqlParameter("ids", NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Uuid)
        {
            Value = ids
        };

        return db.Database.ExecuteSqlRawAsync(
            "SELECT id FROM tb_products WHERE id = ANY(@ids) FOR UPDATE",
            new object[] { param },
            cancellationToken);
    }

    public async Task<OrderResponse> Handle(CheckoutCommand request, CancellationToken cancellationToken)
    {
        var customer = await db.Customers
            .FirstOrDefaultAsync(c => c.UserId == request.UserId, cancellationToken)
            ?? throw new KeyNotFoundException("Customer not found.");

        var cart = await db.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.CustomerId == customer.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Cart not found.");

        if (!cart.Items.Any())
            throw new ArgumentException("Cart is empty.");

        await using var tx = await db.Database.BeginTransactionAsync(cancellationToken);

        var productIds = cart.Items.Select(i => i.ProductId).ToList();

        await AcquireProductLocksAsync(productIds, cancellationToken);

        var products = await db.Products
            .Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, cancellationToken);

        // Collect all insufficient-stock items before throwing
        var insufficient = cart.Items
            .Where(i => products.TryGetValue(i.ProductId, out var p) && p.StockQuantity < i.Quantity)
            .Select(i => products[i.ProductId].Name)
            .ToList();

        if (insufficient.Count > 0)
            throw new UnprocessableEntityException(
                $"Insufficient stock for: {string.Join(", ", insufficient)}.");

        // Deduct stock and build order items
        var orderItems = new List<OrderItem>();
        var total = 0m;

        foreach (var cartItem in cart.Items)
        {
            var product = products[cartItem.ProductId];
            product.StockQuantity -= cartItem.Quantity;
            var unitPrice = product.Price;
            total += unitPrice * cartItem.Quantity;

            orderItems.Add(new OrderItem
            {
                Id = Guid.NewGuid(),
                ProductId = cartItem.ProductId,
                Quantity = cartItem.Quantity,
                UnitPrice = unitPrice,
            });
        }

        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = customer.Id,
            Status = OrderStatus.Pending,
            TotalPrice = total,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Items = orderItems,
        };

        db.Orders.Add(order);
        db.CartItems.RemoveRange(cart.Items);
        try
        {
            await db.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
        }
       
        await tx.CommitAsync(cancellationToken);

        return new OrderResponse(
            order.Id,
            customer.Id,
            customer.Name,
            order.Status,
            order.TotalPrice,
            order.CreatedAt,
            order.UpdatedAt,
            orderItems.Select(i => new OrderItemResponse(i.Id, i.ProductId, i.Quantity, i.UnitPrice)).ToList());
    }
}
