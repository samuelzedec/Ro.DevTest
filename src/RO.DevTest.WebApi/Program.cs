using RO.DevTest.WebApi.Common.Api;

namespace RO.DevTest.WebApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.AddPipeline();

        var app = builder.Build();
        app.UseServices();
        app.Run();
    }
}