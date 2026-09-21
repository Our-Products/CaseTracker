using System;
using System.Threading;
using System.Threading.Tasks;
using CaseTrackerApplication.DTOs.Courts;
using CaseTrackerApplication.Interfaces.Repositories;
using CaseTrackerApplication.Interfaces.Repositories.Cases;
using CaseTrackerApplication.Interfaces.Repositories.Courts;
using CaseTrackerApplication.Interfaces.Repositories.ECourts;
using CaseTrackerApplication.Interfaces.Repositories.Hearings;
using CaseTrackerApplication.Interfaces.Services.Courts;
using CaseTrackerApplication.Interfaces.Services.ECourts;
using CaseTrackerApplication.Services.Courts;
using CaseTrackerApplication.Services.ECourts;
using CaseTrackerInfrastructure.Data;
using CaseTrackerInfrastructure.Repositories.Cases;
using CaseTrackerInfrastructure.Repositories.Courts;
using CaseTrackerInfrastructure.Repositories.ECourts;
using CaseTrackerInfrastructure.Repositories.Hearings;
using CaseTrackerInfrastructure.Services.ECourts;
using CaseTrackerInfrastructure.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureAppConfiguration((hostContext, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        config.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true);
        config.AddEnvironmentVariables();
    })
    .ConfigureServices((hostContext, services) =>
    {
        var configuration = hostContext.Configuration;

        // 1. PostgreSQL Database Configuration
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? configuration["ConnectionStrings:DefaultConnection"]
            ?? configuration["Values:ConnectionStrings:DefaultConnection"]
            ?? configuration["POSTGRESQL_CONNECTION_STRING"]
            ?? "Host=ep-dry-silence-aepi0j4w-pooler.c-2.us-east-2.aws.neon.tech; Database=CaseTracker; Username=neondb_owner; Password=npg_KEhBgumG9Dv6; SSL Mode=VerifyFull; Channel Binding=Require;";

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });

        // 2. Settings & Options
        services.Configure<CourtMasterSyncSettings>(configuration.GetSection("CourtMasterSync"));

        // 3. eCourts HTTP Client
        services.AddHttpClient<IECourtClient, ECourtClient>(client =>
        {
            var baseUrl = configuration["ECourts:BaseUrl"]
                ?? configuration["Values:ECourts:BaseUrl"]
                ?? "https://webapi.ecourtsindia.com";

            client.BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/");
            client.Timeout = TimeSpan.FromSeconds(45);
        });

        // 4. Repositories & Unit Of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ICourtRepository, CourtRepository>();
        services.AddScoped<ICaseRepository, CaseRepository>();
        services.AddScoped<IHearingRepository, HearingRepository>();
        services.AddScoped<IECourtApiLogRepository, ECourtApiLogRepository>();

        // 5. Business Services
        services.AddScoped<ICourtMasterSyncService, CourtMasterSyncService>();
        services.AddScoped<IECourtSyncService, ECourtSyncService>();

        // 6. Initial Startup Synchronization Service
        services.AddHostedService<InitialCourtMasterSyncBackgroundService>();
    })
    .Build();

host.Run();

/// <summary>
/// Hosted background service that triggers initial court master synchronization upon Function App startup
/// when configured or when the court hierarchy tables are empty.
/// </summary>
public class InitialCourtMasterSyncBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly ILogger<InitialCourtMasterSyncBackgroundService> _logger;

    public InitialCourtMasterSyncBackgroundService(
        IServiceProvider serviceProvider,
        IConfiguration configuration,
        ILogger<InitialCourtMasterSyncBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Give the functions host 2 seconds to initialize ports and listeners
        await Task.Delay(2000, stoppingToken);

        var runOnStartupStr = _configuration["CourtMasterSync:RunOnStartup"]
            ?? _configuration["Values:CourtMasterSync:RunOnStartup"]
            ?? "true";
        bool runOnStartup = !string.Equals(runOnStartupStr, "false", StringComparison.OrdinalIgnoreCase);

        if (!runOnStartup)
        {
            _logger.LogInformation("Initial Court Master Sync on startup is disabled via configuration.");
            return;
        }

        try
        {
            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var syncService = scope.ServiceProvider.GetRequiredService<ICourtMasterSyncService>();

            bool hasDistricts = await db.Districts.AnyAsync(stoppingToken);
            bool hasCourts = await db.Courts.AnyAsync(stoppingToken);

            _logger.LogInformation(
                "=== Initial Court Master Sync Initiated on Startup === (Districts in DB: {HasDistricts}, Courts in DB: {HasCourts})",
                hasDistricts, hasCourts);

            var summary = await syncService.SynchronizeAsync(cancellationToken: stoppingToken);

            _logger.LogInformation(
                "=== Initial Court Master Sync Finished ===\n{Summary}",
                summary.ToFormattedSummary());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed during initial Court Master Sync on Function App startup.");
        }
    }
}
