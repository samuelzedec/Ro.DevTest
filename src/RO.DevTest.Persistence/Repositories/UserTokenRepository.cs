using Microsoft.EntityFrameworkCore;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Domain.Entities.Identity;

namespace RO.DevTest.Persistence.Repositories;


public class UserTokenRepository(DefaultContext context) : IUserTokenRepository
{
    public async Task<UserToken?> GetRefreshTokenByUserIdAsync(Guid userId)
        => await context
            .UserTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.UserId == userId);

    public async Task UpdateRefreshTokenAsync(UserToken userToken)
    {
        context.UserTokens.Update(userToken);
        await context.SaveChangesAsync();
    }

    public async Task CreateRefreshTokenAsync(UserToken userToken)
    {
        await context.UserTokens.AddAsync(userToken); 
        await context.SaveChangesAsync();
    }
}