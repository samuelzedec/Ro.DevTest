using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

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
        var optionsBuilder = new DbContextOptionsBuilder<DefaultContext>();
        optionsBuilder.UseNpgsql(
            "Host=localhost;Port=5432;Database=testdev;Username=postgres;Password=1q2w3e4r@#$",
            b => b.MigrationsAssembly("RO.DevTest.Persistence"));
        return new DefaultContext(optionsBuilder.Options);
    }
}