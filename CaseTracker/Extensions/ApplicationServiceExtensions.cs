using CaseTrackerApplication.Interfaces.Services;
using CaseTrackerApplication.Services;

namespace CaseTracker.Extensions
{
    public static class ApplicationServiceExtensions
    {
        /// <summary>
        /// Registers all application layer services with their lifetimes.
        /// </summary>
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Register application services with Scoped lifetime
            // Scoped: New instance per HTTP request
            services.AddScoped<IAuthService, AuthService>();

            return services;
        }
    }
}
