using CaseTrackerApplication.AI;
using CaseTrackerApplication.Interfaces.Repositories;
using CaseTrackerApplication.Interfaces.Services;
using CaseTrackerInfrastructure.AI;
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
            services.AddScoped<CaseTrackerApplication.Interfaces.Repositories.Cases.ICaseRepository, CaseTrackerInfrastructure.Repositories.Cases.CaseRepository>();
            services.AddScoped<CaseTrackerApplication.Interfaces.Repositories.Clients.IClientRepository, CaseTrackerInfrastructure.Repositories.Clients.ClientRepository>();
            services.AddScoped<CaseTrackerApplication.Interfaces.Repositories.Hearings.IHearingRepository, CaseTrackerInfrastructure.Repositories.Hearings.HearingRepository>();
            services.AddScoped<CaseTrackerApplication.Interfaces.Repositories.Courts.ICourtRepository, CaseTrackerInfrastructure.Repositories.Courts.CourtRepository>();
            services.AddScoped<CaseTrackerApplication.Interfaces.Repositories.ECourts.IECourtApiLogRepository, CaseTrackerInfrastructure.Repositories.ECourts.ECourtApiLogRepository>();

            // ==============================
            // UNIT OF WORK
            // ==============================

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // ==============================
            // INFRASTRUCTURE SERVICES
            // ==============================

            services.AddScoped<IPasswordService, PasswordService>();
            services.AddScoped<IJwtService, JwtService>();

            // ==============================
            // ECOURTS EXTERNAL CLIENT
            // ==============================

            services.AddHttpClient<CaseTrackerApplication.Interfaces.Services.ECourts.IECourtClient, CaseTrackerInfrastructure.Services.ECourts.ECourtClient>(client =>
            {
                var baseUrl = configuration["ECourts:BaseUrl"] ?? "https://webapi.ecourtsindia.com";
                client.BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/");
                client.Timeout = TimeSpan.FromSeconds(30);
            });

            // ==============================
            // BACKGROUND JOBS
            // ==============================

            services.AddHostedService<CaseTrackerInfrastructure.BackgroundJobs.CourtMasterSyncWorker>();
            services.AddHostedService<CaseTrackerInfrastructure.BackgroundJobs.CaseStatusSyncWorker>();

            // ==============================
            // AI SERVICES
            // ==============================

            var aiBaseUrl =
                configuration["AI:BaseUrl"]
                ?? throw new InvalidOperationException(
                    "AI base URL is not configured. Add 'AI:BaseUrl' to appsettings.");

            services.AddHttpClient("Ollama", client =>
            {
                client.BaseAddress = new Uri(aiBaseUrl);
                client.Timeout = TimeSpan.FromSeconds(120);
            });

            services.AddScoped<IAIService, OllamaAIService>();

            return services;
        }
    }
}