using System.Threading;
using System.Threading.Tasks;
using CaseTrackerApplication.DTOs.Courts;

namespace CaseTrackerApplication.Interfaces.Services.Courts
{
    public interface ICourtMasterSyncService
    {
        /// <summary>
        /// Synchronizes State -> District -> Court Complex master hierarchy for configured target states (TN, PY).
        /// Idempotent, cost-controlled, and with change-detection.
        /// </summary>
        Task<CourtMasterSyncExecutionSummaryDto> SynchronizeAsync(CourtMasterSyncSettings? settings = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Synchronizes State -> District -> Court Complex for a single state (e.g. TN or PY).
        /// </summary>
        Task<CourtMasterSyncExecutionSummaryDto> SynchronizeStateAsync(string stateCode, CourtMasterSyncSettings? settings = null, CancellationToken cancellationToken = default);
    }
}
