using System;
using System.Threading;
using System.Threading.Tasks;
using CaseTrackerApplication.Interfaces.Services.ECourts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CaseTrackerInfrastructure.BackgroundJobs
{
    /// <summary>
    /// Background service that periodically synchronizes case status, orders,
    /// and hearing histories for pending cases that require updates.
    /// Strictly respects cost limits and controlled batch processing.
    /// </summary>
    public class CaseStatusSyncWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<CaseStatusSyncWorker> _logger;
        private readonly int _syncIntervalHours;
        private readonly int _batchSize;

        public CaseStatusSyncWorker(
            IServiceScopeFactory scopeFactory,
            IConfiguration configuration,
            ILogger<CaseStatusSyncWorker> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;

            _syncIntervalHours = configuration.GetValue("ECourts:Sync:CaseSyncIntervalHours", 24);
            _batchSize = configuration.GetValue("ECourts:Sync:BatchSize", 25);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("CaseStatusSyncWorker initialized. Interval: {Hours}h, BatchSize: {BatchSize}.",
                _syncIntervalHours, _batchSize);

            // Wait 60 seconds after application startup
            await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var syncService = scope.ServiceProvider.GetRequiredService<IECourtSyncService>();

                    _logger.LogInformation("Starting scheduled CaseStatusSync run...");
                    var summary = await syncService.RunScheduledCaseStatusSyncAsync(_batchSize);

                    _logger.LogInformation("Job: {Job} | Completed: {CompletedAt} | TotalEligible: {Total} | Processed: {Processed} | Succeeded: {Succeeded} | Failed: {Failed} | Skipped: {Skipped} | CreditsCharged: {Credits} | Duration: {Duration}ms",
                        summary.JobName, summary.CompletedAt, summary.TotalEligible, summary.Processed, summary.Succeeded, summary.Failed, summary.Skipped, summary.CreditsCharged, summary.DurationMs);

                    if (summary.Errors.Count > 0)
                    {
                        foreach (var err in summary.Errors)
                        {
                            _logger.LogWarning("Sync Notice: {Error}", err);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unhandled error during scheduled CaseStatusSync run.");
                }

                await Task.Delay(TimeSpan.FromHours(_syncIntervalHours), stoppingToken);
            }
        }
    }
}
