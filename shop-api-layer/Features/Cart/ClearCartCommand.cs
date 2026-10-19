using MediatR;
using Microsoft.EntityFrameworkCore;
using shop_api_layer.Data;

namespace shop_api_layer.Features.Cart;

public sealed record ClearCartCommand(Guid UserId) : IRequest;

public sealed class ClearCartHandler(AppDbContext db) : IRequestHandler<ClearCartCommand>
{
    public async Task Handle(ClearCartCommand request, CancellationToken cancellationToken)
    {
        var customer = await db.Customers
            .FirstOrDefaultAsync(c => c.UserId == request.UserId, cancellationToken)
            ?? throw new KeyNotFoundException("Customer not found.");

        var cart = await db.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.CustomerId == customer.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Cart not found.");

        db.CartItems.RemoveRange(cart.Items);
        cart.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(cancellationToken);
    }
}
