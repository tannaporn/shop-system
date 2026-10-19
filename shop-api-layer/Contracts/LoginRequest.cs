using System.ComponentModel.DataAnnotations;

namespace shop_api_layer.Contracts;

public sealed class LoginRequest
{
    [Required, StringLength(128)]
    public string Username { get; init; } = string.Empty;

    [Required, StringLength(1024)]
    public string Password { get; init; } = string.Empty;
}
