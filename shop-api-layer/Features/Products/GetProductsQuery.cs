using MediatR;
using Microsoft.EntityFrameworkCore;
using shop_api_layer.Contracts.Products;
using shop_api_layer.Data;

namespace shop_api_layer.Features.Products;

public sealed record GetProductsQuery(string? Search = null) : IRequest<List<ProductResponse>>;

public sealed class GetProductsHandler(AppDbContext db) : IRequestHandler<GetProductsQuery, List<ProductResponse>>
{
    public async Task<List<ProductResponse>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var query = db.Products.Where(p => p.DeletedAt == null);

        if (!string.IsNullOrWhiteSpace(request.Search))
            query = query.Where(p => p.Name.ToLower().Contains(request.Search.ToLower()));

        return await query
            .OrderBy(p => p.Name)
            .Select(p => new ProductResponse(p.Id, p.Name, p.Description, p.Price, p.StockQuantity, p.CreatedAt, p.UpdatedAt))
            .ToListAsync(cancellationToken);
    }
}
