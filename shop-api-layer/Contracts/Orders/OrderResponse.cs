using shop_api_layer.Enums;

namespace shop_api_layer.Contracts.Orders;

public sealed record OrderItemResponse(
    Guid Id,
    Guid ProductId,
    int Quantity,
    decimal UnitPrice);

public sealed record OrderResponse(
    Guid Id,
    Guid CustomerId,
    string CustomerName,
    OrderStatus Status,
    decimal TotalPrice,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    List<OrderItemResponse> Items);
