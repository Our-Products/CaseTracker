using CaseTrackerApplication.Interfaces.Services;
using CaseTrackerApplication.Services;

namespace CaseTracker.Extensions
{
    public static class ApplicationServiceExtensions
    {
        /// <summary>
        /// Registers all application layer services with their lifetimes.
        /// </summary>
        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services)
        {
            // ==============================
            // APPLICATION SERVICES
            // ==============================

            // Authentication
            services.AddScoped<IAuthService, AuthService>();

            // User
            services.AddScoped<IUserService, UserService>();

            // Role
            services.AddScoped<IRoleService, RoleService>();

            // Lawyer
            services.AddScoped<ILawyerService, LawyerService>();

            // Law Firm
            services.AddScoped<ILawFirmService, LawFirmService>();

            // User - Role
            services.AddScoped<IUserRoleService, UserRoleService>();

            // User - Law Firm
            services.AddScoped<
                IUserLawFirmService,
                UserLawFirmService>();

            return services;
        }
    }
}