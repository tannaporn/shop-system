using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using shop_api_layer.Contracts;
using shop_api_layer.Features.Auth;

namespace shop_api_layer.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IHostEnvironment _environment;
    private readonly IConfiguration _configuration;
    private readonly IOptionsMonitor<JwtBearerOptions> _jwtOptions;
    private readonly IMediator _mediator;

    public AuthController(IHostEnvironment environment, IConfiguration configuration,
        IOptionsMonitor<JwtBearerOptions> jwtOptions, IMediator mediator)
    {
        _environment = environment;
        _configuration = configuration;
        _jwtOptions = jwtOptions;
        _mediator = mediator;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public ActionResult<ApiResponse<LoginResponse>> Login(LoginRequest request)
    {
        Response.Headers.CacheControl = "no-store";
        Response.Headers.Pragma = "no-cache";

        // This single configured account is only for local development, not a production user store.
        if (!_environment.IsDevelopment())
            return NotFound();

        var username = _configuration["DevelopmentLogin:Username"];
        var password = _configuration["DevelopmentLogin:Password"];
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            return NotFound();

        var userMatches = SecretEquals(request.Username, username);
        var passwordMatches = SecretEquals(request.Password, password);
        if (!(userMatches & passwordMatches))
            return Problem(statusCode: StatusCodes.Status401Unauthorized,
                detail: "Invalid username or password.");

        var validation = _jwtOptions.Get(JwtBearerDefaults.AuthenticationScheme).TokenValidationParameters;
        var now = DateTimeOffset.UtcNow;
        var expiresAt = now.AddMinutes(30);
        var token = new JwtSecurityToken(
            issuer: validation.ValidIssuer,
            audience: validation.ValidAudience,
            claims: new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            },
            notBefore: now.UtcDateTime,
            expires: expiresAt.UtcDateTime,
            signingCredentials: new SigningCredentials(validation.IssuerSigningKey, SecurityAlgorithms.HmacSha256));

        return Ok(new ApiResponse<LoginResponse>(new LoginResponse(
            new JwtSecurityTokenHandler().WriteToken(token), "Bearer", expiresAt)));
    }

    private static bool SecretEquals(string actual, string expected) =>
        CryptographicOperations.FixedTimeEquals(
            SHA256.HashData(Encoding.UTF8.GetBytes(actual)),
            SHA256.HashData(Encoding.UTF8.GetBytes(expected)));

    [HttpGet("me")]
    [ProducesResponseType(typeof(ApiResponse<GetCurrentUserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<GetCurrentUserResponse>>> GetMe()
    {
        var email = User.FindFirstValue("email") ?? User.FindFirstValue(ClaimTypes.Email);
        var sub = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(sub))
            return Problem(statusCode: StatusCodes.Status400BadRequest, detail: "Missing email or sub claim.");

        var result = await _mediator.Send(new GetCurrentUserQuery { Email = email, Sub = sub });
        return Ok(new ApiResponse<GetCurrentUserResponse>(result));
    }
}
