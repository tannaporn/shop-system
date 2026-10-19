using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using shop_api_layer.Contracts.Cart;
using shop_api_layer.Data;

namespace shop_api_layer.Features.Cart;

public sealed record UpdateCartItemCommand(Guid CartItemId, int Quantity, Guid UserId) : IRequest<CartResponse>;

public sealed class UpdateCartItemHandler(AppDbContext db, IConfiguration? config = null) : IRequestHandler<UpdateCartItemCommand, CartResponse>
{
    public async Task<CartResponse> Handle(UpdateCartItemCommand request, CancellationToken cancellationToken)
    {
        if (request.Quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.");

        var customer = await db.Customers
            .FirstOrDefaultAsync(c => c.UserId == request.UserId, cancellationToken)
            ?? throw new KeyNotFoundException("Customer not found.");

        var cart = await db.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.CustomerId == customer.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Cart not found.");

        var item = cart.Items.FirstOrDefault(i => i.Id == request.CartItemId)
            ?? throw new KeyNotFoundException("Cart item not found.");

        var product = await db.Products
            .FirstOrDefaultAsync(p => p.Id == item.ProductId && p.DeletedAt == null, cancellationToken)
            ?? throw new KeyNotFoundException("Product not found.");

        if (request.Quantity > product.StockQuantity)
            throw new ArgumentException($"Quantity exceeds stock. Available: {product.StockQuantity}.");

        item.Quantity = request.Quantity;
        item.UpdatedAt = DateTime.UtcNow;
        cart.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(cancellationToken);

        return await new GetCartHandler(db, config).Handle(new GetCartQuery(request.UserId), cancellationToken);
    }
}
