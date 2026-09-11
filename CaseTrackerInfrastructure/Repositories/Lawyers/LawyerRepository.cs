using CaseTrackerApplication.Interfaces.Repositories;
using CaseTrackerDomain.Models;
using CaseTrackerInfrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CaseTrackerInfrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for Lawyer entity.
    /// Encapsulates all database access logic for lawyer management.
    /// </summary>
    public class LawyerRepository : Repository<Lawyer>, ILawyerRepository
    {
        public LawyerRepository(ApplicationDbContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Get Lawyer by UserId asynchronously.
        /// </summary>
        public async Task<Lawyer?> GetByUserIdAsync(Guid userId)
        {
            return await _dbSet
                .Include(x => x.User)
                .Include(x => x.LawFirm)
                .FirstOrDefaultAsync(x => x.UserId == userId);
        }

        /// <summary>
        /// Get Lawyer by Bar Council ID asynchronously.
        /// </summary>
        public async Task<Lawyer?> GetByBarCouncilIdAsync(
            string barCouncilId)
        {
            return await _dbSet
                .Include(x => x.User)
                .Include(x => x.LawFirm)
                .FirstOrDefaultAsync(x =>
                    x.BarCouncilId == barCouncilId);
        }

        /// <summary>
        /// Get all lawyers associated with a specific law firm asynchronously.
        /// </summary>
        public async Task<IEnumerable<Lawyer>> GetByLawFirmIdAsync(
            Guid lawFirmId)
        {
            return await _dbSet
                .Where(x => x.LawFirmId == lawFirmId)
                .Include(x => x.User)
                .OrderBy(x => x.FullName)
                .ToListAsync();
        }

        /// <summary>
        /// Get all active lawyers asynchronously.
        /// </summary>
        public async Task<IEnumerable<Lawyer>> GetAllActiveAsync()
        {
            return await _dbSet
                .Where(x => x.Status == "Active")
                .Include(x => x.User)
                .Include(x => x.LawFirm)
                .OrderBy(x => x.FullName)
                .ToListAsync();
        }

        /// <summary>
        /// Search lawyers by full name asynchronously.
        /// </summary>
        public async Task<IEnumerable<Lawyer>> SearchByNameAsync(
            string fullName)
        {
            return await _dbSet
                .Where(x => x.FullName.Contains(fullName))
                .Include(x => x.User)
                .Include(x => x.LawFirm)
                .OrderBy(x => x.FullName)
                .ToListAsync();
        }
    }
}