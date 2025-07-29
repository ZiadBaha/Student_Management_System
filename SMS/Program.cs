using SMS.Repository.Data.Context;
using SMS.Repository.Data.Seed;
using SMS.Setup;


public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        ServiceConfigurator.Configure(builder.Services, builder.Configuration);

        var app = builder.Build();

        try
        {
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                await SeedRolesInitializer.SeedRolesAsync(services);
                await SeedAdminInitializer.SeedAdminAsync(services);

                var context = services.GetRequiredService<StoreContext>();
                var logger = services.GetRequiredService<ILogger<Program>>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error during seeding: " + ex.Message);
        }




        MiddlewareConfigurator.Configure(app, builder.Environment);

        app.Run();
    }
}