using Microsoft.OpenApi.Models;
using System.Reflection;

namespace CaseTracker.Extensions
{
    public static class SwaggerServiceExtensions
    {
        /// <summary>
        /// Configures Swagger documentation with JWT Bearer token authentication.
        /// </summary>
        public static IServiceCollection AddSwaggerDocumentation(
            this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "CaseTracker API",
                    Version = "v1",
                    Description = "Enterprise Legal Practice Management & Case Tracking API with Role-Based Access Control."
                });

                // Configure JWT Bearer Security Scheme
                var securityScheme = new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "JWT Authorization header using the Bearer scheme. Enter your JWT token in the text input below. Swagger will automatically prefix 'Bearer ' to requests.",
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

                // Require Bearer token for protected endpoints
                var securityRequirement = new OpenApiSecurityRequirement
                {
                    {
                        securityScheme,
                        Array.Empty<string>()
                    }
                };

                options.AddSecurityRequirement(securityRequirement);

                // Include XML Comments if generated
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    options.IncludeXmlComments(xmlPath);
                }
            });

            return services;
        }
    }
}
