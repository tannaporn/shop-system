using MediatR;
using Microsoft.EntityFrameworkCore;
using shop_api_layer.Contracts.Orders;
using shop_api_layer.Data;
using shop_api_layer.Enums;

namespace shop_api_layer.Features.Orders;

public sealed record GetOrdersQuery(OrderStatus? Status = null) : IRequest<List<OrderResponse>>;

public sealed class GetOrdersHandler(AppDbContext db) : IRequestHandler<GetOrdersQuery, List<OrderResponse>>
{
    public async Task<List<OrderResponse>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        var query = db.Orders
            .Include(o => o.Items)
            .Join(db.Customers,
                o => o.CustomerId,
                c => c.Id,
                (o, c) => new { Order = o, CustomerName = c.Name });

        if (request.Status.HasValue)
            query = query.Where(x => x.Order.Status == request.Status.Value);

        return await query
            .OrderByDescending(x => x.Order.CreatedAt)
            .Select(x => new OrderResponse(
                x.Order.Id,
                x.Order.CustomerId,
                x.CustomerName,
                x.Order.Status,
                x.Order.TotalPrice,
                x.Order.CreatedAt,
                x.Order.UpdatedAt,
                x.Order.Items.Select(i => new OrderItemResponse(i.Id, i.ProductId, i.Quantity, i.UnitPrice)).ToList()))
            .ToListAsync(cancellationToken);
    }
}
