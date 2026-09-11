using CaseTracker.Constants;
using Microsoft.AspNetCore.Authorization;

namespace CaseTracker.Extensions
{
    public static class AuthorizationServiceExtensions
    {
        /// <summary>
        /// Registers authorization policies and services.
        /// </summary>
        public static IServiceCollection AddCustomAuthorization(
            this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                // Every protected endpoint requires authentication
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();

                // Company Level (SuperAdmin only)
                options.AddPolicy("SuperAdminOnly", policy =>
                {
                    policy.RequireRole(AppRoles.SuperAdmin);
                });

                // Firm Level Admin or Company SuperAdmin
                options.AddPolicy("AdminOnly", policy =>
                {
                    policy.RequireRole(AppRoles.SuperAdmin, AppRoles.Admin);
                });

                // Lawyer only
                options.AddPolicy("LawyerOnly", policy =>
                {
                    policy.RequireRole(AppRoles.Lawyer);
                });

                // SuperAdmin, Firm Admin, or Lawyer
                options.AddPolicy("LawyerOrAdmin", policy =>
                {
                    policy.RequireRole(AppRoles.SuperAdmin, AppRoles.Admin, AppRoles.Lawyer);
                });

                // Staff only
                options.AddPolicy("StaffOnly", policy =>
                {
                    policy.RequireRole(AppRoles.Staff);
                });
            });

            return services;
        }
    }
}