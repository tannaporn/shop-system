namespace shop_api_layer.Contracts;

public sealed record ApiError(
    string Code,
    string Message,
    string TraceId,
    IDictionary<string, string[]>? Errors = null);
