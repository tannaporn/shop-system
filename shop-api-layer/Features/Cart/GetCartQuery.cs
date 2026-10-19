using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using shop_api_layer.Contracts.Cart;
using shop_api_layer.Data;
using shop_api_layer.Models.Cart;
using shop_api_layer.Models.Customer;

namespace shop_api_layer.Features.Cart;

public sealed record GetCartQuery(Guid UserId) : IRequest<CartResponse>;

public sealed class GetCartHandler(AppDbContext db, IConfiguration? config = null) : IRequestHandler<GetCartQuery, CartResponse>
{
    public async Task<CartResponse> Handle(GetCartQuery request, CancellationToken cancellationToken)
    {
        // Upsert customer
        var customer = await db.Customers
            .FirstOrDefaultAsync(c => c.UserId == request.UserId, cancellationToken);

        if (customer is null)
        {
            // dev mode: lookup by email from config; fallback to UserId lookup for tests
            var devEmail = config?["DevUser:Email"];
            var userRole = devEmail != null
                ? await db.UserRoles.FirstOrDefaultAsync(u => u.Email == devEmail, cancellationToken)
                : await db.UserRoles.FirstOrDefaultAsync(u => u.UserId == request.UserId, cancellationToken);

            if (userRole is null) throw new KeyNotFoundException("User not found in whitelist.");

            customer = new Customer
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Name = userRole!.Email,
                Email = userRole.Email,
                CreatedAt = DateTime.UtcNow,
            };
            db.Customers.Add(customer);
            await db.SaveChangesAsync(cancellationToken);
        }

        // Upsert cart
        var cart = await db.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.CustomerId == customer.Id, cancellationToken);

        if (cart is null)
        {
            cart = new Models.Cart.Cart
            {
                Id = Guid.NewGuid(),
                CustomerId = customer.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };
            db.Carts.Add(cart);
            await db.SaveChangesAsync(cancellationToken);
        }

        // Build response with product info
        var productIds = cart.Items.Select(i => i.ProductId).ToList();
        var products = await db.Products
            .Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, cancellationToken);

        var items = cart.Items.Select(i =>
        {
            products.TryGetValue(i.ProductId, out var product);
            var unitPrice = product?.Price ?? 0m;
            return new CartItemResponse(
                i.Id,
                i.ProductId,
                product?.Name ?? string.Empty,
                i.Quantity,
                unitPrice,
                unitPrice * i.Quantity);
        }).ToList();

        return new CartResponse(cart.Id, items, items.Sum(i => i.Subtotal));
    }
}
