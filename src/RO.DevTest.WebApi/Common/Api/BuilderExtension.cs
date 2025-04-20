using System.Text.Json.Serialization;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NSwag;
using NSwag.Generation.Processors.Security;
using RO.DevTest.Application;
using RO.DevTest.Application.Contracts.Infrastructure;
using RO.DevTest.Application.Settings;
using RO.DevTest.Infrastructure.IoC;
using RO.DevTest.Infrastructure.Services;
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
        builder.Services.AddValidatorsFromAssembly(typeof(ApplicationLayer).Assembly);
        
        builder.Services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssemblies(
                typeof(ApplicationLayer).Assembly,
                typeof(Program).Assembly
            ));

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddControllers();
        builder.Services.Configure<ApiBehaviorOptions>(options 
            => options.SuppressModelStateInvalidFilter = true);
        builder.Services.Configure<JsonOptions>(options 
            => options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
        
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddOpenApiDocument(document =>
        {
            document.Title = "RO.DevTest API";
            document.Version = "v1";
            document.Description = "API for RO.DevTest application";
            
            document.AddSecurity("Bearer", new OpenApiSecurityScheme
            {
                Description = "Insira o token JWT. Exemplo: \"Bearer {seu_token}\"",
                Name = "Authorization",
                In = OpenApiSecurityApiKeyLocation.Header,
                Type = OpenApiSecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT"
            });

            document.OperationProcessors.Add(
                new AspNetCoreOperationSecurityScopeProcessor("Bearer"));
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
}