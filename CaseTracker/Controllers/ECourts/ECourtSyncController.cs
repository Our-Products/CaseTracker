using System;
using System.Threading.Tasks;
using CaseTracker.Constants;
using CaseTrackerApplication.DTOs.Common;
using CaseTrackerApplication.DTOs.ECourts;
using CaseTrackerApplication.Interfaces.Services.Courts;
using CaseTrackerApplication.Interfaces.Services.ECourts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CaseTracker.Controllers.ECourts
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = AppRoles.SuperAdminOrAdmin)]
    public class ECourtSyncController : ControllerBase
    {
        private readonly IECourtSyncService _syncService;
        private readonly ICourtMasterService _courtMasterService;

        public ECourtSyncController(
            IECourtSyncService syncService,
            ICourtMasterService courtMasterService)
        {
            _syncService = syncService;
            _courtMasterService = courtMasterService;
        }

        /// <summary>
        /// Manually triggers a batch case status synchronization run for eligible pending cases.
        /// </summary>
        [HttpPost("sync/cases")]
        public async Task<IActionResult> SyncCases([FromQuery] int? batchSize)
        {
            var result = await _syncService.RunScheduledCaseStatusSyncAsync(batchSize);
            return Ok(ApiResponse<ECourtSyncResultDto>.SuccessResponse(result, "Case status sync execution finished."));
        }

        /// <summary>
        /// Manually triggers a court master synchronization for Tamil Nadu (TN) or Puducherry (PY).
        /// Zero credit cost endpoint.
        /// </summary>
        [HttpPost("sync/court-master")]
        public async Task<IActionResult> SyncCourtMaster([FromQuery] string stateCode = "TN")
        {
            var result = await _courtMasterService.SynchronizeMasterDataAsync(stateCode);
            return Ok(ApiResponse<CourtMasterSyncResultDto>.SuccessResponse(result, "Court master synchronization finished."));
        }

        /// <summary>
        /// Manually triggers upcoming hearing synchronization for active cases.
        /// </summary>
        [HttpPost("sync/hearings")]
        public async Task<IActionResult> SyncHearings([FromQuery] int? batchSize, [FromQuery] int daysAhead = 14)
        {
            var result = await _syncService.RunScheduledHearingSyncAsync(batchSize, daysAhead);
            return Ok(ApiResponse<ECourtSyncResultDto>.SuccessResponse(result, "Upcoming hearing sync execution finished."));
        }

        /// <summary>
        /// Retrieves eCourts API credit consumption and usage metrics for a given date range.
        /// </summary>
        [HttpGet("sync/usage")]
        public async Task<IActionResult> GetUsage([FromQuery] DateTimeOffset? from, [FromQuery] DateTimeOffset? to)
        {
            var fromDate = from ?? DateTimeOffset.UtcNow.AddDays(-30);
            var toDate = to ?? DateTimeOffset.UtcNow;

            var summary = await _syncService.GetApiUsageSummaryAsync(fromDate, toDate);
            return Ok(ApiResponse<ECourtUsageSummaryDto>.SuccessResponse(summary));
        }
    }
}
