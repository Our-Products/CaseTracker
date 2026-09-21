using System;
using System.Threading;
using System.Threading.Tasks;
using CaseTrackerApplication.Interfaces.Services.Courts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CaseTrackerInfrastructure.BackgroundJobs
{
    /// <summary>
    /// Background service that periodically synchronizes Court Master data
    /// (States, Districts, Court Complexes, Courts) for Tamil Nadu (TN) and Puducherry (PY).
    /// Target schedule: Monthly, zero credit cost.
    /// </summary>
    public class CourtMasterSyncWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<CourtMasterSyncWorker> _logger;
        private static readonly string[] TargetStates = { "TN", "PY" };

        public CourtMasterSyncWorker(
            IServiceScopeFactory scopeFactory,
            ILogger<CourtMasterSyncWorker> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("CourtMasterSyncWorker initialized. Scheduled for Tamil Nadu and Puducherry master hierarchy.");

            // Wait 30 seconds after startup before the initial run
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                foreach (var state in TargetStates)
                {
                    if (stoppingToken.IsCancellationRequested) break;

                    try
                    {
                        using var scope = _scopeFactory.CreateScope();
                        var masterService = scope.ServiceProvider.GetRequiredService<ICourtMasterService>();

                        _logger.LogInformation("Starting scheduled Court Master synchronization for state {State}...", state);
                        var result = await masterService.SynchronizeMasterDataAsync(state);
                        _logger.LogInformation("Court Master sync for {State} completed in {Duration}ms. Result: {Message}",
                            state, result.DurationMs, result.Message);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Unhandled error during Court Master sync for state {State}", state);
                    }
                }

                // Run approximately once every 30 days
                await Task.Delay(TimeSpan.FromDays(30), stoppingToken);
            }
        }
    }
}
