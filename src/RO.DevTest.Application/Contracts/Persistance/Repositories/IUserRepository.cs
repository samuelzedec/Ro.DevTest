using System.Linq.Expressions;
using RO.DevTest.Domain.Entities;
using RO.DevTest.Domain.Entities.Identity;

namespace RO.DevTest.Application.Contracts.Persistance.Repositories;

public interface IUserRepository : IBaseRepository<User>;
