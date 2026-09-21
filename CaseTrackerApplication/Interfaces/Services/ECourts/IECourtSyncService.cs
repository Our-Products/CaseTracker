using System;
using System.Threading.Tasks;
using CaseTrackerApplication.DTOs.ECourts;

namespace CaseTrackerApplication.Interfaces.Services.ECourts
{
    public interface IECourtSyncService
    {
        Task<ECourtSyncResultDto> RunScheduledCaseStatusSyncAsync(int? batchSizeOverride = null);
        Task<ECourtSyncResultDto> RunScheduledHearingSyncAsync(int? batchSizeOverride = null, int daysAhead = 14);
        Task<ECourtUsageSummaryDto> GetApiUsageSummaryAsync(DateTimeOffset from, DateTimeOffset to);
    }
}
