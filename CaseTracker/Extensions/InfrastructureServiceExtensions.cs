<<<<<<< Updated upstream
using Application.Interfaces;
using Domain.Data;
using Infrastructure.Repositories;
=======
using CaseTrackerInfrastructure.Data;
>>>>>>> Stashed changes
using Microsoft.EntityFrameworkCore;

namespace CaseTracker.Extensions
{
    public static class InfrastructureServiceExtensions
    {
        /// <summary>
        /// Registers all infrastructure layer services including DbContext and repositories.
        /// </summary>
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services, 
            IConfiguration configuration)
        {
            // Register DbContext with Scoped lifetime
            // Scoped: New instance per HTTP request
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString));

            // Register repositories with Scoped lifetime
            // Scoped: New instance per HTTP request

            return services;
        }
    }
}
