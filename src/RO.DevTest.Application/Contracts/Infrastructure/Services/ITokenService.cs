using RO.DevTest.Domain.Entities.Identity;

namespace RO.DevTest.Application.Contracts.Infrastructure.Services;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    Task<bool> ValidationRefreshTokenAsync(User user);
    Task<string> CreateRefreshTokenAsync(User user);
}