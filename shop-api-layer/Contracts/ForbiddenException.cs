namespace shop_api_layer.Contracts;

/// <summary>Exception that maps to HTTP 403 Forbidden.</summary>
public sealed class ForbiddenException(string message) : Exception(message);
