using System.Linq.Expressions;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Domain.Entities;

namespace RO.DevTest.Persistence.Repositories;

public class UserRepository(DefaultContext context)
    : BaseRepository<User>(context), IUserRepository
{
    public async Task<User> CreateAsync(User user, CancellationToken cancellationToken = default)
        => await base.CreateAsync(user, cancellationToken);

    public async Task UpdateAsync(User user, CancellationToken cancellationToken)
        => await base.UpdateAsync(user, cancellationToken);

    public async Task DeleteAsync(User user, CancellationToken cancellationToken)
        => await base.DeleteAsync(user, cancellationToken);
    
    public async Task<User?> GetAsync(
        CancellationToken cancellationToken,
        Expression<Func<User, bool>> predicate,
        params Expression<Func<User, object>>[] includes
    )
        => await base.GetAsync(cancellationToken, predicate, includes);
}