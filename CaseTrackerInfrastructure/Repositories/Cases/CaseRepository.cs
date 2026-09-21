using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CaseTrackerApplication.Interfaces.Repositories.Cases;
using CaseTrackerDomain.Models;
using CaseTrackerInfrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CaseTrackerInfrastructure.Repositories.Cases
{
    public class CaseRepository : Repository<Case>, ICaseRepository
    {
        public CaseRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Case?> GetByCnrAsync(string cnrNumber)
        {
            return await _dbSet
                .Include(c => c.Court)
                .FirstOrDefaultAsync(c => c.CnrNumber == cnrNumber);
        }

        public async Task<Case?> GetWithDetailsAsync(Guid caseId)
        {
            return await _dbSet
                .Include(c => c.Court)
                .Include(c => c.CaseClients)
                    .ThenInclude(cc => cc.Client)
                .Include(c => c.CaseLawyers)
                    .ThenInclude(cl => cl.Lawyer)
                .Include(c => c.CaseHearings)
                .Include(c => c.CaseOrders)
                .FirstOrDefaultAsync(c => c.CaseId == caseId);
        }

        public async Task<IEnumerable<Case>> GetCasesByLawFirmAsync(Guid lawFirmId)
        {
            return await _dbSet
                .Include(c => c.Court)
                .Where(c => c.LawFirmId == lawFirmId && c.Status == "Active")
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Case>> GetCasesByCourtAsync(Guid courtId)
        {
            return await _dbSet
                .Where(c => c.CourtId == courtId && c.Status == "Active")
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Case>> GetEligibleForSyncAsync(int batchSize, int syncIntervalHours)
        {
            var cutoff = DateTimeOffset.UtcNow.AddHours(-syncIntervalHours);

            return await _dbSet
                .Where(c => c.IsEcourtSynced &&
                            c.CnrNumber != null &&
                            c.CaseStatus != "Disposed" &&
                            c.CaseStatus != "Dismissed" &&
                            c.Status == "Active" &&
                            (c.LastSyncedAt == null || c.LastSyncedAt < cutoff))
                .OrderBy(c => c.LastSyncedAt ?? DateTimeOffset.MinValue)
                .Take(batchSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Case>> GetEligibleForHearingSyncAsync(int batchSize, int daysAhead = 14)
        {
            var today = DateTime.UtcNow.Date;
            var futureLimit = today.AddDays(daysAhead);

            var casesWithUpcomingHearings = await _dbSet
                .Include(c => c.CaseHearings)
                .Where(c => c.IsEcourtSynced &&
                            c.CnrNumber != null &&
                            c.CaseStatus != "Disposed" &&
                            c.CaseStatus != "Dismissed" &&
                            c.Status == "Active" &&
                            c.CaseHearings.Any(h => h.HearingDate >= today && h.HearingDate <= futureLimit))
                .OrderBy(c => c.LastSyncedAt ?? DateTimeOffset.MinValue)
                .Take(batchSize)
                .ToListAsync();

            if (casesWithUpcomingHearings.Count >= batchSize)
            {
                return casesWithUpcomingHearings;
            }

            int remaining = batchSize - casesWithUpcomingHearings.Count;
            var existingIds = casesWithUpcomingHearings.Select(c => c.CaseId).ToList();

            var additionalCases = await _dbSet
                .Include(c => c.CaseHearings)
                .Where(c => c.IsEcourtSynced &&
                            c.CnrNumber != null &&
                            c.CaseStatus != "Disposed" &&
                            c.CaseStatus != "Dismissed" &&
                            c.Status == "Active" &&
                            !existingIds.Contains(c.CaseId))
                .OrderBy(c => c.LastSyncedAt ?? DateTimeOffset.MinValue)
                .Take(remaining)
                .ToListAsync();

            return casesWithUpcomingHearings.Concat(additionalCases).ToList();
        }

        public async Task<IEnumerable<CaseLawyer>> GetCaseLawyersAsync(Guid caseId)
        {
            return await _context.Set<CaseLawyer>()
                .Include(cl => cl.Lawyer)
                .Where(cl => cl.CaseId == caseId && cl.Status == "Active")
                .ToListAsync();
        }

        public async Task AddCaseLawyerAsync(CaseLawyer caseLawyer)
        {
            var existing = await _context.Set<CaseLawyer>()
                .FirstOrDefaultAsync(cl => cl.CaseId == caseLawyer.CaseId && cl.LawyerId == caseLawyer.LawyerId);

            if (existing != null)
            {
                existing.Status = "Active";
                existing.LawyerRoleId = caseLawyer.LawyerRoleId;
                existing.AssignedAt = caseLawyer.AssignedAt;
                existing.UpdatedBy = caseLawyer.UpdatedBy;
            }
            else
            {
                await _context.Set<CaseLawyer>().AddAsync(caseLawyer);
            }
        }

        public async Task RemoveCaseLawyerAsync(Guid caseId, Guid lawyerId)
        {
            var existing = await _context.Set<CaseLawyer>()
                .FirstOrDefaultAsync(cl => cl.CaseId == caseId && cl.LawyerId == lawyerId);

            if (existing != null)
            {
                _context.Set<CaseLawyer>().Remove(existing);
            }
        }
    }
}
