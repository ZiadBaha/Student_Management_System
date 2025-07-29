using SMS.Extensions;
using SMS.Middlewares;

namespace SMS.Setup
{
    public static class MiddlewareConfigurator
    {
        public static void Configure(WebApplication app, IWebHostEnvironment env)
        {
            //if (env.IsDevelopment())
            //{
            app.UseSwaggerDocumentation();
            //}

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCors("AllowFrontend");

            app.UseMiddleware<GlobalExceptionMiddleware>();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCustomMiddlewares();
            app.MapControllers();
        }
    }
}
