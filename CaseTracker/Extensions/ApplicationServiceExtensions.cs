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

            // Legal Matters & Core Operations
            services.AddScoped<CaseTrackerApplication.Interfaces.Services.Cases.ICaseService, CaseTrackerApplication.Services.Cases.CaseService>();
            services.AddScoped<CaseTrackerApplication.Interfaces.Services.Clients.IClientService, CaseTrackerApplication.Services.Clients.ClientService>();
            services.AddScoped<CaseTrackerApplication.Interfaces.Services.Hearings.IHearingService, CaseTrackerApplication.Services.Hearings.HearingService>();
            services.AddScoped<CaseTrackerApplication.Interfaces.Services.Courts.ICourtMasterService, CaseTrackerApplication.Services.Courts.CourtMasterService>();
            services.AddScoped<CaseTrackerApplication.Interfaces.Services.Courts.ICourtMasterSyncService, CaseTrackerApplication.Services.Courts.CourtMasterSyncService>();
            services.AddScoped<CaseTrackerApplication.Interfaces.Services.ECourts.IECourtSyncService, CaseTrackerApplication.Services.ECourts.ECourtSyncService>();

            return services;
        }
    }
}