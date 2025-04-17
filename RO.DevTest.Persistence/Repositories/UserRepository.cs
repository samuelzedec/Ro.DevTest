using System.Linq.Expressions;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Domain.Entities;
using RO.DevTest.Domain.Entities.Identity;

namespace RO.DevTest.Persistence.Repositories;

public class UserRepository(DefaultContext context)
    : BaseRepository<User>(context), IUserRepository;