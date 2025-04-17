namespace RO.DevTest.WebApi.Common.Api;

public static class AppExtension
{
    public static void UseServices(this WebApplication app)
    {
        app.UseSecurity();
        app.UseDevelopmentEnvironment();
        app.MapControllers();
        app.Run();
    }
    
    private static void UseDevelopmentEnvironment(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

    }
    
    private static void UseSecurity(this WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
    }
}