using MediatR;
using Microsoft.EntityFrameworkCore;
using shop_api_layer.Contracts;
using shop_api_layer.Data;

namespace shop_api_layer.Features.Auth
{
    public class GetCurrentUserQuery : IRequest<GetCurrentUserResponse>
    {
        public required string Email { get; set; }
        public required string Sub { get; set; }

        public class Handler : IRequestHandler<GetCurrentUserQuery, GetCurrentUserResponse>
        {
            private readonly AppDbContext _db;

            public Handler(AppDbContext db)
            {
                _db = db;
            }

            public async Task<GetCurrentUserResponse> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
            {
                var userRole = await _db.UserRoles
                    .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

                if (userRole is null)
                    throw new ForbiddenException("Email is not in the whitelist.");

                if (userRole.UserId is null)
                {
                    userRole.UserId = Guid.Parse(request.Sub);
                    await _db.SaveChangesAsync(cancellationToken);
                }

                return new GetCurrentUserResponse(userRole.UserId!.Value, userRole.Email, userRole.Role);
            }
        }
    }

    public record GetCurrentUserResponse(Guid UserId, string Email, string Role);
}
