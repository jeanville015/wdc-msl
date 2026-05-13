using IDM.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace IDM.Infrastructure.Repositories
{
    public class UserContext : IUserContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid GetUserId()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            var userIdStr = user?.FindFirst("sub")?.Value // JWT "sub" claim
                          ?? user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (Guid.TryParse(userIdStr, out var guid))
            {
                return guid;
            }

            // fallback for system jobs, seeds, etc.
            return Guid.Empty;
        }

        public string GetUserName()
        {
            return _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "system";
        }
    }
}
