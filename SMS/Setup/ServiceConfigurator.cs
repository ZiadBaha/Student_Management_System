using SMS.Core.Models.Account;
using SMS.Extensions;
using SMS.Filters;

namespace SMS.Setup
{
    public static class ServiceConfigurator
    {
        public static void Configure(IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers(options =>
            {
                options.Filters.Add<ValidationFilter>();
            })
            .AddNewtonsoftJson();

            services.AddEndpointsApiExplorer();
            services.AddSwaggerDocumentation();

            //  Add Identity & JWT + DbContext
            services.AddIdentityServices(configuration);

            //  Add DI for Application Services (Repositories, Helpers...)
            services.AddApplicationServices(configuration);

            //  FluentValidation registration
            services.AddFluentValidationServices();

            //  Mail settings configuration
            services.Configure<MailSettings>(configuration.GetSection(nameof(MailSettings)));


            services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

        }
    }
}
