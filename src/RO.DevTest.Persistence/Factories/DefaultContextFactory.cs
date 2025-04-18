using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace RO.DevTest.Persistence.Factories;

/// <summary>
/// A interface <see cref="IDesignTimeDbContextFactory{T}"/> é utilizada para configurar o DbContext
/// em tempo de design, permitindo que as migrações e outros comandos do EF Core
/// sejam executados sem depender do pipeline de execução da aplicação.
/// </summary>
public class DefaultContextFactory : IDesignTimeDbContextFactory<DefaultContext>
{
    public DefaultContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../RO.DevTest.WebApi"))
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
            
        var optionsBuilder = new DbContextOptionsBuilder<DefaultContext>();
        optionsBuilder.UseNpgsql(
            configuration.GetConnectionString("DefaultConnection"),
            b => b.MigrationsAssembly("RO.DevTest.Persistence"));
        return new DefaultContext(optionsBuilder.Options);
    }
}