using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Persistence.Repositories;

namespace RO.DevTest.Persistence.IoC;

public static class PersistenceDependencyInjector
{
    /// <summary>
    /// Inject the dependencies of the Persistence layer into an
    /// <see cref="IServiceCollection"/>
    /// </summary>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> to inject the dependencies into
    /// </param>
    /// <returns>
    /// The <see cref="IServiceCollection"/> with dependencies injected
    /// </returns>
    public static void InjectPersistenceDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<DefaultContext>(options =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly("RO.DevTest.Persistence"));
        });

        services.AddTransient<IUserRepository, UserRepository>();
        
    }
}