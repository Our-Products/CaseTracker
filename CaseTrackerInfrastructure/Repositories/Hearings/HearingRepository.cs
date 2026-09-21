using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CaseTrackerApplication.Interfaces.Repositories.Hearings;
using CaseTrackerDomain.Models;
using CaseTrackerInfrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CaseTrackerInfrastructure.Repositories.Hearings
{
    public class HearingRepository : Repository<CaseHearing>, IHearingRepository
    {
        public HearingRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<CaseHearing>> GetHearingsByCaseAsync(Guid caseId)
        {
            return await _dbSet
                .Where(h => h.CaseId == caseId)
                .OrderByDescending(h => h.HearingDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<CaseHearing>> GetDailyBoardAsync(DateTime date, Guid? lawFirmId, Guid? courtId)
        {
            var q = _dbSet
                .Include(h => h.Case)
                .Where(h => h.HearingDate.Date == date.Date && h.Case != null && h.Case.Status == "Active");

            if (lawFirmId.HasValue)
            {
                q = q.Where(h => h.Case!.LawFirmId == lawFirmId.Value);
            }

            if (courtId.HasValue)
            {
                q = q.Where(h => h.Case!.CourtId == courtId.Value);
            }

            return await q
                .OrderBy(h => h.ItemNumber ?? int.MaxValue)
                .ThenBy(h => h.CourtHall)
                .ToListAsync();
        }

        public async Task<CaseHearing?> GetLatestHearingForCaseAsync(Guid caseId)
        {
            return await _dbSet
                .Where(h => h.CaseId == caseId)
                .OrderByDescending(h => h.HearingDate)
                .FirstOrDefaultAsync();
        }
    }
}
