namespace shop_api_layer.Exceptions;

public sealed class UnprocessableEntityException(string message) : Exception(message);
