using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using RO.DevTest.Application.Contracts.Infrastructure.Services;

namespace RO.DevTest.Infrastructure.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public string GetCurrentUserId()
    {
        var userId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        return userId ?? string.Empty;
    }

    public bool IsAdmin()
        => httpContextAccessor.HttpContext?.User.IsInRole("Admin") ?? false;
}