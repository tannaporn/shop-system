namespace shop_api_layer.Contracts.Products;

public sealed record ProductResponse(
    Guid Id,
    string Name,
    string? Description,
    decimal Price,
    int StockQuantity,
    DateTime CreatedAt,
    DateTime UpdatedAt);
