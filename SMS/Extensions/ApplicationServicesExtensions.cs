using AutoMapper;
using Microsoft.AspNetCore.Cors.Infrastructure;
using SMS.Core.Services;
using SMS.Helpers;
using SMS.Repository.Services;



namespace SMS.Extensions
{
    public static class ApplicationServicesExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient();

            services.AddAutoMapper(typeof(MappingProfile).Assembly);
            //services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

   

            // Register Account & Authentication Services
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IOtpService, OtpService>();
            services.AddScoped<ITokenService, TokenService>();

            // Email
            services.AddScoped<IEmailService, EmailService>();


            // For admin 
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IDepartmentService, DepartmentService>();
            services.AddScoped<IStudentService, StudentService>();



            return services;
        }
    }
}
