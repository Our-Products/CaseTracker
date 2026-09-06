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
                options.UseNpgsql(connectionString);
            });

            // Register repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ILawFirmRepository, LawFirmRepository>();
            services.AddScoped<ILawyerRepository, LawyerRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUserLawFirmRepository, UserLawFirmRepository>();
            services.AddScoped<IUserRoleRepository, UserRoleRepository>();

            // Register infrastructure services
            services.AddScoped<IAuthService, AuthService>();

            return services;
        }
    }
}
