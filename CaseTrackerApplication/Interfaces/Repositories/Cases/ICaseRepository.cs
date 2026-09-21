using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CaseTrackerDomain.Models;

namespace CaseTrackerApplication.Interfaces.Repositories.Cases
{
    public interface ICaseRepository : IRepository<Case>
    {
        Task<Case?> GetByCnrAsync(string cnrNumber);
        Task<Case?> GetWithDetailsAsync(Guid caseId);
        Task<IEnumerable<Case>> GetCasesByLawFirmAsync(Guid lawFirmId);
        Task<IEnumerable<Case>> GetCasesByCourtAsync(Guid courtId);
        Task<IEnumerable<Case>> GetEligibleForSyncAsync(int batchSize, int syncIntervalHours);
        Task<IEnumerable<Case>> GetEligibleForHearingSyncAsync(int batchSize, int daysAhead = 14);
        Task<IEnumerable<CaseLawyer>> GetCaseLawyersAsync(Guid caseId);
        Task AddCaseLawyerAsync(CaseLawyer caseLawyer);
        Task RemoveCaseLawyerAsync(Guid caseId, Guid lawyerId);
    }
}
