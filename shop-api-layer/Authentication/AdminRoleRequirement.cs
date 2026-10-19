using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using shop_api_layer.Data;

namespace shop_api_layer.Authentication;

public sealed class AdminRoleRequirement : IAuthorizationRequirement;

public sealed class AdminRoleHandler(AppDbContext db) : AuthorizationHandler<AdminRoleRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context, AdminRoleRequirement requirement)
    {
        var email = context.User.FindFirst(JwtRegisteredClaimNames.Email)?.Value
            ?? context.User.FindFirst("email")?.Value;

        if (string.IsNullOrEmpty(email))
            return;

        var isAdmin = await db.UserRoles
            .AnyAsync(u => u.Email == email && u.Role == "admin");

        if (isAdmin)
            context.Succeed(requirement);
    }
}
