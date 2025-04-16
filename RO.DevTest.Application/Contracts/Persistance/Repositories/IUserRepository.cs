using System.Linq.Expressions;
using RO.DevTest.Domain.Entities;

namespace RO.DevTest.Application.Contracts.Persistance.Repositories;

public interface IUserRepository : IBaseRepository<User>
{
    Task<User> CreateAsync(User user, CancellationToken cancellationToken);
    Task UpdateAsync(User user, CancellationToken cancellationToken);
    Task DeleteAsync(User user, CancellationToken cancellationToken);

    Task<User?> GetAsync(CancellationToken cancellationToken, Expression<Func<User, bool>> predicate,
        params Expression<Func<User, object>>[] includes);
}
