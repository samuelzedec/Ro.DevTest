using RO.DevTest.Domain.Entities.Identity;

namespace RO.DevTest.Domain.Services;

public interface ITokenService
{
    string GenerateToken(User user);
    string GenerateRefreshToken();
}