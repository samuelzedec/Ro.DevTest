using RO.DevTest.Application;
using RO.DevTest.Infrastructure.IoC;
using RO.DevTest.Persistence.IoC;

namespace RO.DevTest.WebApi.Common.Api;

public static class BuilderExtension
{
    public static void AddPipeline(this WebApplicationBuilder builder)
    {
        builder.AddServices();
    }
    
    private static void AddServices(this WebApplicationBuilder builder)
    {
        builder.Services.InjectPersistenceDependencies(builder.Configuration);
        builder.Services.InjectInfrastructureDependencies();
        
        builder.Services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssemblies(
                typeof(ApplicationLayer).Assembly,
                typeof(Program).Assembly
            ));

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        
        builder.Services.AddSwaggerGen();
    }
}