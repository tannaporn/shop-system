using MediatR;
using Microsoft.EntityFrameworkCore;
using shop_api_layer.Contracts.Products;
using shop_api_layer.Data;

namespace shop_api_layer.Features.Products;

public sealed record UpdateStockCommand(Guid Id, int StockQuantity) : IRequest<ProductResponse>;

public sealed class UpdateStockHandler(AppDbContext db) : IRequestHandler<UpdateStockCommand, ProductResponse>
{
    public async Task<ProductResponse> Handle(UpdateStockCommand request, CancellationToken cancellationToken)
    {
        if (request.StockQuantity < 0)
            throw new ArgumentException("Stock quantity cannot be negative.");

        var product = await db.Products
            .Where(p => p.Id == request.Id && p.DeletedAt == null)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new KeyNotFoundException($"Product {request.Id} was not found.");

        product.StockQuantity = request.StockQuantity;
        product.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync(cancellationToken);

        return new ProductResponse(product.Id, product.Name, product.Description, product.Price, product.StockQuantity, product.CreatedAt, product.UpdatedAt);
    }
}
