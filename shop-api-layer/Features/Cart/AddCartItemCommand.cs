using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using shop_api_layer.Contracts;
using shop_api_layer.Contracts.Cart;
using shop_api_layer.Data;
using shop_api_layer.Models.Cart;

namespace shop_api_layer.Features.Cart;

public sealed record AddCartItemCommand(Guid UserId, Guid ProductId, int Quantity) : IRequest<CartResponse>;

public sealed class AddCartItemHandler(AppDbContext db, IConfiguration? config = null) : IRequestHandler<AddCartItemCommand, CartResponse>
{
    public async Task<CartResponse> Handle(AddCartItemCommand request, CancellationToken cancellationToken)
    {
        if (request.Quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.");

        var product = await db.Products
            .FirstOrDefaultAsync(p => p.Id == request.ProductId && p.DeletedAt == null, cancellationToken)
            ?? throw new KeyNotFoundException("Product not found.");

        if (product.StockQuantity <= 0)
            throw new ArgumentException("Product is out of stock.");

        var customer = await db.Customers
            .FirstOrDefaultAsync(c => c.UserId == request.UserId, cancellationToken);

        if (customer is null)
        {
            var devEmail = config?["DevUser:Email"];
            var userRole = devEmail != null
                ? await db.UserRoles.FirstOrDefaultAsync(u => u.Email == devEmail, cancellationToken)
                : await db.UserRoles.FirstOrDefaultAsync(u => u.UserId == request.UserId, cancellationToken);

            if (userRole is null) throw new ForbiddenException("User not found in whitelist.");

            customer = new Models.Customer.Customer
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Name = userRole.Email,
                Email = userRole.Email,
                CreatedAt = DateTime.UtcNow,
            };
            db.Customers.Add(customer);
            await db.SaveChangesAsync(cancellationToken);
        }

        var cart = await db.Carts
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
        }

        var existing = await db.CartItems
            .FirstOrDefaultAsync(i => i.CartId == cart.Id && i.ProductId == request.ProductId, cancellationToken);

        if (existing is not null)
        {
            var merged = existing.Quantity + request.Quantity;
            if (merged > product.StockQuantity)
                throw new ArgumentException($"Cannot add {request.Quantity} more. Only {product.StockQuantity - existing.Quantity} left in stock.");

            existing.Quantity = merged;
            existing.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            if (request.Quantity > product.StockQuantity)
                throw new ArgumentException($"Quantity exceeds stock. Available: {product.StockQuantity}.");

            db.CartItems.Add(new CartItem
            {
                Id = Guid.NewGuid(),
                CartId = cart.Id,
                ProductId = request.ProductId,
                Quantity = request.Quantity,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            });
        }

        await db.SaveChangesAsync(cancellationToken);

        return await new GetCartHandler(db, config).Handle(new GetCartQuery(request.UserId), cancellationToken);
    }
}
