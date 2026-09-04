using CaseTrackerApplication.Interfaces;
using CaseTrackerInfrastructure.Data;
using CaseTrackerInfrastructure.Repositories;
using CaseTrackerInfrastructure.Services;
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
            var useVirtualDb = configuration.GetValue<bool>("UseVirtualDatabase");
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var virtualConnectionString = configuration.GetConnectionString("VirtualConnection") ?? "Data Source=casetracker_virtual_dev.db";

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                if (useVirtualDb || string.IsNullOrWhiteSpace(connectionString))
                {
                    options.UseSqlite(virtualConnectionString);
                }
                else
                {
                    options.UseNpgsql(connectionString);
                }
            });

            // Register repositories
            services.AddScoped<IUserRepository, UserRepository>();

            // Register infrastructure services
            services.AddScoped<IAuthService, AuthService>();

            return services;
        }
    }
}
