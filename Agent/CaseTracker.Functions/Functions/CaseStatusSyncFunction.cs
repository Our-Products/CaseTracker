using System;
using System.Threading;
using System.Threading.Tasks;
using CaseTrackerApplication.Interfaces.Services.ECourts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace CaseTracker.Functions.Functions
{
    public class CaseStatusSyncFunction
    {
        private readonly IECourtSyncService _syncService;
        private readonly ILogger<CaseStatusSyncFunction> _logger;

        public CaseStatusSyncFunction(
            IECourtSyncService syncService,
            ILogger<CaseStatusSyncFunction> logger)
        {
            _syncService = syncService;
            _logger = logger;
        }

        /// <summary>
        /// Scheduled Timer Trigger executing on a configured schedule (e.g., daily at 03:00 UTC).
        /// Synchronizes status for active, eCourts-synced cases.
        /// </summary>
        [Function("CaseStatusSyncTimer")]
        public async Task RunTimerAsync(
            [TimerTrigger("%CaseStatusSync:Schedule%")] TimerInfo timerInfo,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Scheduled CaseStatusSyncTimer triggered at: {Time}", DateTimeOffset.UtcNow);

            try
            {
                var result = await _syncService.RunScheduledCaseStatusSyncAsync();

                _logger.LogInformation(
                    "Case Status Sync completed. Processed: {Processed}, Succeeded: {Succeeded}, Failed: {Failed}, Skipped: {Skipped}",
                    result.Processed, result.Succeeded, result.Failed, result.Skipped);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error during scheduled Case Status Sync execution.");
                throw;
            }
        }

        /// <summary>
        /// Safe manual HTTP Trigger for on-demand case status synchronization.
        /// Usage: POST /api/case-status-sync?batchSize=25
        /// </summary>
        [Function("CaseStatusSyncManual")]
        public async Task<IActionResult> RunManualAsync(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "case-status-sync")] HttpRequest req,
            CancellationToken cancellationToken)
        {
            string? batchQuery = req.Query["batchSize"];
            int? batchSize = null;
            if (int.TryParse(batchQuery, out int parsedBatch) && parsedBatch > 0)
            {
                batchSize = parsedBatch;
            }

            _logger.LogInformation("Manual Case Status Sync triggered. BatchSize: {BatchSize}", batchSize?.ToString() ?? "Default (25)");

            var result = await _syncService.RunScheduledCaseStatusSyncAsync(batchSize);

            return new OkObjectResult(new
            {
                success = result.Failed == 0,
                jobName = result.JobName,
                message = result.Message,
                data = result
            });
        }
    }
}
