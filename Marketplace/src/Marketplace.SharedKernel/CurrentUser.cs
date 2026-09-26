using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Marketplace.SharedKernel;

public sealed class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid UserId
    {
        get
        {
            var httpContext = _httpContextAccessor.HttpContext
                ?? throw new InvalidOperationException("No active HTTP context.");
            
            var sub = httpContext.User.FindFirstValue("sub")
                ?? throw new InvalidOperationException("Authenticated request missing sub claim.");
            
            return Guid.Parse(sub);
        }
    }
}