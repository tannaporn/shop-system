using MediatR;
using shop_api_layer.Contracts.Products;
using shop_api_layer.Data;
using shop_api_layer.Models.Product;

namespace shop_api_layer.Features.Products;

public sealed record CreateProductCommand(
    string Name,
    string? Description,
    decimal Price,
    int StockQuantity) : IRequest<ProductResponse>;

public sealed class CreateProductHandler(AppDbContext db) : IRequestHandler<CreateProductCommand, ProductResponse>
{
    public async Task<ProductResponse> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            StockQuantity = request.StockQuantity,
            CreatedAt = now,
            UpdatedAt = now,
        };

        db.Products.Add(product);
        await db.SaveChangesAsync(cancellationToken);

        return new ProductResponse(product.Id, product.Name, product.Description, product.Price, product.StockQuantity, product.CreatedAt, product.UpdatedAt);
    }
}
