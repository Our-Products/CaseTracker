using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CaseTrackerDomain.Models;

namespace CaseTrackerApplication.Interfaces.Repositories.Hearings
{
    public interface IHearingRepository : IRepository<CaseHearing>
    {
        Task<IEnumerable<CaseHearing>> GetHearingsByCaseAsync(Guid caseId);
        Task<IEnumerable<CaseHearing>> GetDailyBoardAsync(DateTime date, Guid? lawFirmId, Guid? courtId);
        Task<CaseHearing?> GetLatestHearingForCaseAsync(Guid caseId);
    }
}
