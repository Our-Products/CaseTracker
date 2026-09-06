using CaseTrackerApplication.Interfaces;
using CaseTrackerDomain.Models;
using CaseTrackerInfrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CaseTrackerInfrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for LawFirm entity.
    /// Encapsulates all database access logic for law firm management.
    /// </summary>
    public class LawFirmRepository : ILawFirmRepository
    {
        private readonly ApplicationDbContext _context;

        public LawFirmRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get LawFirm by registration number asynchronously.
        /// </summary>
        public async Task<LawFirm?> GetByRegistrationNumberAsync(string registrationNumber)
        {
            return await _context.LawFirms
                .FirstOrDefaultAsync(x => x.RegistrationNumber == registrationNumber);
        }

        /// <summary>
        /// Get all active law firms asynchronously.
        /// </summary>
        public async Task<IEnumerable<LawFirm>> GetAllActiveAsync()
        {
            return await _context.LawFirms
                .Where(x => x.CreatedAt != default)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Search law firms by name asynchronously.
        /// </summary>
        public async Task<IEnumerable<LawFirm>> SearchByNameAsync(string firmName)
        {
            return await _context.LawFirms
                .Where(x => x.FirmName.Contains(firmName))
                .OrderBy(x => x.FirmName)
                .ToListAsync();
        }
    }
}
