using CaseTrackerApplication.Interfaces.Repositories;
using CaseTrackerApplication.Interfaces.Services;
using CaseTrackerInfrastructure.Data;
using CaseTrackerInfrastructure.Repositories;
using CaseTrackerInfrastructure.Services;
using CaseTrackerInfrastructure.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace CaseTracker.Extensions
{
    public static class InfrastructureServiceExtensions
    {
        /// <summary>
        /// Registers all infrastructure layer services including
        /// DbContext, repositories, UnitOfWork and infrastructure services.
        /// </summary>
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var connectionString =
                configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "Database connection string 'DefaultConnection' is not configured.");

            // ==============================
            // DATABASE
            // ==============================

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });

            // ==============================
            // REPOSITORIES
            // ==============================

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ILawFirmRepository, LawFirmRepository>();
            services.AddScoped<ILawyerRepository, LawyerRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<
                IUserLawFirmRepository,
                UserLawFirmRepository>();
            services.AddScoped<
                IUserRoleRepository,
                UserRoleRepository>();

            // ==============================
            // UNIT OF WORK
            // ==============================

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // ==============================
            // INFRASTRUCTURE SERVICES
            // ==============================

            services.AddScoped<IPasswordService, PasswordService>();
            services.AddScoped<IJwtService, JwtService>();

            return services;
        }
    }
}