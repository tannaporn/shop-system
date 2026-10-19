using MediatR;
using Microsoft.EntityFrameworkCore;
using shop_api_layer.Contracts.Products;
using shop_api_layer.Data;

namespace shop_api_layer.Features.Products;

public sealed record UpdateProductCommand(
    Guid Id,
    string Name,
    string? Description,
    decimal Price) : IRequest<ProductResponse>;

public sealed class UpdateProductHandler(AppDbContext db) : IRequestHandler<UpdateProductCommand, ProductResponse>
{
    public async Task<ProductResponse> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await db.Products
            .Where(p => p.Id == request.Id && p.DeletedAt == null)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new KeyNotFoundException($"Product {request.Id} was not found.");

        product.Name = request.Name;
        product.Description = request.Description;
        product.Price = request.Price;
        product.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync(cancellationToken);

        return new ProductResponse(product.Id, product.Name, product.Description, product.Price, product.StockQuantity, product.CreatedAt, product.UpdatedAt);
    }
}
