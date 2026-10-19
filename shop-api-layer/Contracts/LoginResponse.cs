namespace shop_api_layer.Contracts;

public sealed record LoginResponse(string AccessToken, string TokenType, DateTimeOffset ExpiresAtUtc);
