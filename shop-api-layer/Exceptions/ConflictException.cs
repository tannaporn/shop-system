namespace shop_api_layer.Exceptions;

public sealed class ConflictException(string message) : Exception(message);
