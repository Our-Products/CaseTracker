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
    public class HearingSyncFunction
    {
        private readonly IECourtSyncService _syncService;
        private readonly ILogger<HearingSyncFunction> _logger;

        public HearingSyncFunction(
            IECourtSyncService syncService,
            ILogger<HearingSyncFunction> logger)
        {
            _syncService = syncService;
            _logger = logger;
        }

        /// <summary>
        /// Scheduled Timer Trigger executing on a configured schedule (e.g., daily at 04:00 UTC).
        /// Synchronizes upcoming hearing dates and dockets for active cases.
        /// </summary>
        [Function("HearingSyncTimer")]
        public async Task RunTimerAsync(
            [TimerTrigger("%HearingSync:Schedule%")] TimerInfo timerInfo,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Scheduled HearingSyncTimer triggered at: {Time}", DateTimeOffset.UtcNow);

            try
            {
                var result = await _syncService.RunScheduledHearingSyncAsync();

                _logger.LogInformation(
                    "Hearing Sync completed. Processed: {Processed}, Succeeded: {Succeeded}, Failed: {Failed}, Skipped: {Skipped}",
                    result.Processed, result.Succeeded, result.Failed, result.Skipped);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error during scheduled Hearing Sync execution.");
                throw;
            }
        }

        /// <summary>
        /// Safe manual HTTP Trigger for on-demand hearing synchronization.
        /// Usage: POST /api/hearing-sync?batchSize=25&daysAhead=14
        /// </summary>
        [Function("HearingSyncManual")]
        public async Task<IActionResult> RunManualAsync(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "hearing-sync")] HttpRequest req,
            CancellationToken cancellationToken)
        {
            string? batchQuery = req.Query["batchSize"];
            int? batchSize = null;
            if (int.TryParse(batchQuery, out int parsedBatch) && parsedBatch > 0)
            {
                batchSize = parsedBatch;
            }

            string? daysQuery = req.Query["daysAhead"];
            int daysAhead = 14;
            if (int.TryParse(daysQuery, out int parsedDays) && parsedDays > 0)
            {
                daysAhead = parsedDays;
            }

            _logger.LogInformation("Manual Hearing Sync triggered. BatchSize: {BatchSize}, DaysAhead: {DaysAhead}",
                batchSize?.ToString() ?? "Default (25)", daysAhead);

            var result = await _syncService.RunScheduledHearingSyncAsync(batchSize, daysAhead);

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
