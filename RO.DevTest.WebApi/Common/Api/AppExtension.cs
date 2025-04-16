namespace RO.DevTest.WebApi.Common.Api;

public static class AppExtension
{
    public static void RegisterServices(this WebApplication app)
    {
        app.UseSecurity();
        app.UseDevelopmentEnvironment();
        app.MapControllers();
        app.Run();
    }
    
    private static void UseDevelopmentEnvironment(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment()) return;
        app.UseSwagger();
        app.UseSwaggerUI();
        app.MapSwagger().RequireAuthorization();
    }
    
    private static void UseSecurity(this WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
    }
}