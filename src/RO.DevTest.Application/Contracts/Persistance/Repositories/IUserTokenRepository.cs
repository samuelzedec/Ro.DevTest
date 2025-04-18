using RO.DevTest.Domain.Entities.Identity;

namespace RO.DevTest.Application.Contracts.Persistance.Repositories;

public interface IUserTokenRepository
{
    Task<UserToken?> GetRefreshTokenByUserIdAsync(Guid userId);
    Task UpdateRefreshTokenAsync(UserToken userToken);
    Task CreateRefreshTokenAsync(UserToken userToken);
}