using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using RO.DevTest.Application.Contracts.Infrastructure.Services;

namespace RO.DevTest.Infrastructure.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public Guid GetCurrentGuidId()
    {
        var userId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException("Usuário não autenticado");

        return Guid.Parse(userId);
    }

    public bool IsAdmin()
        => httpContextAccessor.HttpContext?.User.IsInRole("Admin") ?? false;
}