using CaseTrackerApplication.Interfaces.Repositories;
using CaseTrackerDomain.Models;
using CaseTrackerInfrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CaseTrackerInfrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for LawFirm entity.
    /// </summary>
    public class LawFirmRepository
        : Repository<LawFirm>, ILawFirmRepository
    {
        public LawFirmRepository(ApplicationDbContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Get LawFirm by registration number asynchronously.
        /// </summary>
        public async Task<LawFirm?> GetByRegistrationNumberAsync(
            string registrationNumber)
        {
            return await _dbSet
                .FirstOrDefaultAsync(x =>
                    x.RegistrationNumber == registrationNumber);
        }

        /// <summary>
        /// Get all active law firms asynchronously.
        /// </summary>
        public async Task<IEnumerable<LawFirm>> GetAllActiveAsync()
        {
            return await _dbSet
                .Where(x => x.Status == "Active")
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Search law firms by name asynchronously.
        /// </summary>
        public async Task<IEnumerable<LawFirm>> SearchByNameAsync(
            string firmName)
        {
            return await _dbSet
                .Where(x => x.FirmName.Contains(firmName))
                .OrderBy(x => x.FirmName)
                .ToListAsync();
        }
    }
}