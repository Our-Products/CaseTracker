using CaseTrackerApplication.Interfaces;
using CaseTrackerDomain.Models;
using CaseTrackerInfrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CaseTrackerInfrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for UserLawFirm junction entity.
    /// Encapsulates all database access logic for user-law firm associations.
    /// </summary>
    public class UserLawFirmRepository : IUserLawFirmRepository
    {
        private readonly ApplicationDbContext _context;

        public UserLawFirmRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all law firms associated with a specific user asynchronously.
        /// </summary>
        public async Task<IEnumerable<UserLawFirm>> GetByUserIdAsync(Guid userId)
        {
            return await _context.UserLawFirms
                .Where(x => x.UserId == userId)
                .Include(x => x.User)
                .Include(x => x.LawFirm)
                .ToListAsync();
        }

        /// <summary>
        /// Get all users associated with a specific law firm asynchronously.
        /// </summary>
        public async Task<IEnumerable<UserLawFirm>> GetByLawFirmIdAsync(Guid lawFirmId)
        {
            return await _context.UserLawFirms
                .Where(x => x.LawFirmId == lawFirmId)
                .Include(x => x.User)
                .Include(x => x.LawFirm)
                .ToListAsync();
        }

        /// <summary>
        /// Get a specific UserLawFirm association asynchronously.
        /// </summary>
        public async Task<UserLawFirm?> GetByUserAndLawFirmAsync(Guid userId, Guid lawFirmId)
        {
            return await _context.UserLawFirms
                .Include(x => x.User)
                .Include(x => x.LawFirm)
                .FirstOrDefaultAsync(x => x.UserId == userId && x.LawFirmId == lawFirmId);
        }

        /// <summary>
        /// Check if a user is associated with a law firm asynchronously.
        /// </summary>
        public async Task<bool> IsUserInLawFirmAsync(Guid userId, Guid lawFirmId)
        {
            return await _context.UserLawFirms
                .AnyAsync(x => x.UserId == userId && x.LawFirmId == lawFirmId);
        }

        /// <summary>
        /// Get all active UserLawFirm associations asynchronously.
        /// </summary>
        public async Task<IEnumerable<UserLawFirm>> GetAllActiveAsync()
        {
            return await _context.UserLawFirms
                .Where(x => x.Status == "Active")
                .Include(x => x.User)
                .Include(x => x.LawFirm)
                .ToListAsync();
        }
    }
}
