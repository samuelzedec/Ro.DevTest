using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using RO.DevTest.Application;
using RO.DevTest.Application.Services;
using RO.DevTest.Application.Settings;
using RO.DevTest.Domain.Services;
using RO.DevTest.Infrastructure.IoC;
using RO.DevTest.Persistence.IoC;
using Serilog;
using Serilog.Events;

namespace RO.DevTest.WebApi.Common.Api;

public static class BuilderExtension
{
    public static void AddPipeline(this WebApplicationBuilder builder)
    {
        builder.AddLogs();
        builder.AddSettings();
        builder.AddServices();
        builder.AddSecurity();
        builder.AddInversionDependency();
    }
    
    private static void AddLogs(this WebApplicationBuilder builder)
    {
        var output = "{Timestamp:dd-MM-yyyy HH:mm:ss} [{Level}] {Message} {Properties:j}{NewLine}{Exception}";
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console(outputTemplate: output, restrictedToMinimumLevel: LogEventLevel.Information)
            .WriteTo.File("Logs/application-{Date}-logs.txt", restrictedToMinimumLevel: LogEventLevel.Error,
                rollingInterval: RollingInterval.Day)
            .CreateLogger();
        builder.Logging.AddSerilog();
    }
    
    private static void AddSettings(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<JwtSettings>(
            builder.Configuration.GetSection("Jwt"));
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
        
        builder.Services.AddOpenApiDocument(document =>
        {
            document.Title = "RO.DevTest API";
            document.Version = "v1";
            document.Description = "API for RO.DevTest application";
        });
    }
    
    private static void AddSecurity(this WebApplicationBuilder builder)
    {
        // Cria logger específico para autenticação usando Serilog
        var logger = LoggerFactory.Create(loggingBuilder => 
            loggingBuilder.AddSerilog()).CreateLogger("Authentication");
        
        var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>() 
            ?? throw new InvalidOperationException("JWT settings are not configured properly");
        
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new X509SecurityKey(jwtSettings.GenerateCertificate()),
                ValidateIssuer = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtSettings.Audience
            };

            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    logger.LogError(context.Exception.Message);
                    return Task.CompletedTask;
                }
            };
        });
        builder.Services.AddAuthorization();
    }
    
    private static void AddInversionDependency(this WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<ITokenService, TokenService>();
    }
}