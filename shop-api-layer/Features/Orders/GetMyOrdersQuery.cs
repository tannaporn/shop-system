using MediatR;
using Microsoft.EntityFrameworkCore;
using shop_api_layer.Contracts.Orders;
using shop_api_layer.Data;

namespace shop_api_layer.Features.Orders;

public sealed record GetMyOrdersQuery(Guid UserId) : IRequest<List<OrderResponse>>;

public sealed class GetMyOrdersHandler(AppDbContext db) : IRequestHandler<GetMyOrdersQuery, List<OrderResponse>>
{
    public async Task<List<OrderResponse>> Handle(GetMyOrdersQuery request, CancellationToken cancellationToken)
    {
        var customer = await db.Customers.FirstOrDefaultAsync(c => c.UserId == request.UserId, cancellationToken)
            ?? throw new KeyNotFoundException("Customer not found.");

        return await db.Orders
            .Include(o => o.Items)
            .Where(o => o.CustomerId == customer.Id)
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => new OrderResponse(
                o.Id,
                o.CustomerId,
                customer.Name,
                o.Status,
                o.TotalPrice,
                o.CreatedAt,
                o.UpdatedAt,
                o.Items.Select(i => new OrderItemResponse(i.Id, i.ProductId, i.Quantity, i.UnitPrice)).ToList()))
            .ToListAsync(cancellationToken);
    }
}
