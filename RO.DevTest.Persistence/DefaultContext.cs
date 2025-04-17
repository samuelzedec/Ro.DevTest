using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RO.DevTest.Domain.Entities.Identity;

namespace RO.DevTest.Persistence;

public class DefaultContext(DbContextOptions<DefaultContext> options)
    : IdentityDbContext<
        User,
        Role,
        Guid,
        UserClaim,
        UserRole,
        UserLogin,
        RoleClaim,
        UserToken
    >(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.ApplyConfigurationsFromAssembly(typeof(DefaultContext).Assembly);
        builder.HasPostgresExtension("uuid-ossp");
    }
}