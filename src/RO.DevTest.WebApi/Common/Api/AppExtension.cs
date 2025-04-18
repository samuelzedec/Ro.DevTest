namespace RO.DevTest.WebApi.Common.Api;

public static class AppExtension
{
    public static void UseServices(this WebApplication app)
    {
        app.UseSecurity();
        app.UseDevelopmentEnvironment();
        app.MapControllers();
    }
    
    private static void UseDevelopmentEnvironment(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseOpenApi();
            app.UseSwaggerUi();
        }

    }
    
    private static void UseSecurity(this WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
    }
}