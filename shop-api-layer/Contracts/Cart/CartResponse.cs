namespace shop_api_layer.Contracts.Cart;

public sealed record CartItemResponse(
    Guid Id,
    Guid ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal Subtotal);

public sealed record CartResponse(
    Guid CartId,
    List<CartItemResponse> Items,
    decimal Total);
