using Microsoft.OpenApi.Models;


namespace SMS.Extensions
{
    public static class SwaggerServiceExtensions
    {
        public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
        {
            try
            {
                services.AddSwaggerGen(options =>
                {
                    options.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Title = "Student Management System",
                        Version = "v1",
                        Description = "API for managing the Student management system."
                    });

                    // JWT Authentication setup
                    var securityScheme = new OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        Description = "Enter JWT Bearer token in the format: Bearer {token}",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    };

                    options.AddSecurityDefinition("Bearer", securityScheme);

                    options.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            securityScheme,
                            new[] { "Bearer" }
                        }
                    });
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(" Swagger Generation Failed: " + ex.Message);
            }

            return services;
        }

        public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app)
        {
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Student Management System ");
                //c.RoutePrefix = string.Empty;
                c.RoutePrefix = "swagger";

                c.DocumentTitle = "SMS API Docs";
            });

            return app;
        }
    }
}
