using System;
using System.Threading;
using System.Threading.Tasks;
using CaseTrackerApplication.DTOs.Courts;
using CaseTrackerApplication.Interfaces.Services.Courts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace CaseTracker.Functions.Functions
{
    public class CourtMasterSyncFunction
    {
        private readonly ICourtMasterSyncService _syncService;
        private readonly ILogger<CourtMasterSyncFunction> _logger;

        public CourtMasterSyncFunction(
            ICourtMasterSyncService syncService,
            ILogger<CourtMasterSyncFunction> logger)
        {
            _syncService = syncService;
            _logger = logger;
        }

        /// <summary>
        /// Scheduled Timer Trigger executing once per month (configurable via CourtMasterSync:Schedule).
        /// Synchronizes State -> District -> Court Complex hierarchy for Tamil Nadu and Puducherry.
        /// </summary>
        [Function("CourtMasterSyncTimer")]
        public async Task RunTimerAsync(
            [TimerTrigger("%CourtMasterSync:Schedule%")] TimerInfo timerInfo,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Scheduled CourtMasterSyncTimer trigger executed at: {Time}", DateTimeOffset.UtcNow);

            try
            {
                var summary = await _syncService.SynchronizeAsync(cancellationToken: cancellationToken);

                _logger.LogInformation(
                    "Scheduled Court Master Sync completed.\n{Summary}",
                    summary.ToFormattedSummary());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error during scheduled Court Master Sync execution.");
                throw;
            }
        }

        /// <summary>
        /// Manual HTTP Trigger for on-demand synchronization, testing, or development verification.
        /// Usage: GET or POST /api/court-master-sync?state=TN
        /// </summary>
        [Function("CourtMasterSyncManual")]
        public async Task<IActionResult> RunManualAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "court-master-sync")] HttpRequest req,
            CancellationToken cancellationToken)
        {
            string? stateQuery = req.Query["state"];
            string? maxRequestsQuery = req.Query["maxRequests"];

            var settings = new CourtMasterSyncSettings();
            if (int.TryParse(maxRequestsQuery, out int maxReqs) && maxReqs > 0)
            {
                settings.MaxApiRequestsPerRun = maxReqs;
            }

            _logger.LogInformation("Manual Court Master Sync triggered. TargetState: {State}, MaxRequests: {Max}",
                stateQuery ?? "All (TN, PY)", settings.MaxApiRequestsPerRun);

            CourtMasterSyncExecutionSummaryDto summary;

            if (!string.IsNullOrWhiteSpace(stateQuery))
            {
                summary = await _syncService.SynchronizeStateAsync(stateQuery.Trim().ToUpperInvariant(), settings, cancellationToken);
            }
            else
            {
                summary = await _syncService.SynchronizeAsync(settings, cancellationToken);
            }

            return new OkObjectResult(new
            {
                success = summary.Status == "Completed" || summary.Status == "Partial",
                status = summary.Status,
                message = summary.ToFormattedSummary(),
                data = summary
            });
        }
    }
}
