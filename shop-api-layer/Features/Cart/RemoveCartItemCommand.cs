using MediatR;
using Microsoft.EntityFrameworkCore;
using shop_api_layer.Data;

namespace shop_api_layer.Features.Cart;

public sealed record RemoveCartItemCommand(Guid CartItemId, Guid UserId) : IRequest;

public sealed class RemoveCartItemHandler(AppDbContext db) : IRequestHandler<RemoveCartItemCommand>
{
    public async Task Handle(RemoveCartItemCommand request, CancellationToken cancellationToken)
    {
        var customer = await db.Customers
            .FirstOrDefaultAsync(c => c.UserId == request.UserId, cancellationToken)
            ?? throw new KeyNotFoundException("Customer not found.");

        var cart = await db.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.CustomerId == customer.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Cart not found.");

        var item = cart.Items.FirstOrDefault(i => i.Id == request.CartItemId)
            ?? throw new KeyNotFoundException("Cart item not found.");

        db.CartItems.Remove(item);
        cart.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(cancellationToken);
    }
}
