using CaseTrackerApplication.Interfaces.Repositories;
using CaseTrackerDomain.Models;
using CaseTrackerInfrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CaseTrackerInfrastructure.Repositories
{
    public class UserLawFirmRepository
        : Repository<UserLawFirm>, IUserLawFirmRepository
    {
        private readonly ApplicationDbContext _context;

        public UserLawFirmRepository(ApplicationDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserLawFirm>> GetByUserIdAsync(
            Guid userId)
        {
            return await _dbSet
                .Where(x => x.UserId == userId)
                .Include(x => x.User)
                .Include(x => x.LawFirm)
                .ToListAsync();
        }

        public async Task<IEnumerable<UserLawFirm>> GetByLawFirmIdAsync(
            Guid lawFirmId)
        {
            return await _dbSet
                .Where(x => x.LawFirmId == lawFirmId)
                .Include(x => x.User)
                .Include(x => x.LawFirm)
                .ToListAsync();
        }

        public async Task<UserLawFirm?> GetByUserAndLawFirmAsync(
            Guid userId,
            Guid lawFirmId)
        {
            return await _dbSet
                .Include(x => x.User)
                .Include(x => x.LawFirm)
                .FirstOrDefaultAsync(x =>
                    x.UserId == userId &&
                    x.LawFirmId == lawFirmId);
        }

        public async Task<bool> IsUserInLawFirmAsync(
            Guid userId,
            Guid lawFirmId)
        {
            return await _dbSet
                .AnyAsync(x =>
                    x.UserId == userId &&
                    x.LawFirmId == lawFirmId);
        }

        /// <summary>
        /// Get all active UserLawFirm associations asynchronously.
        /// </summary>
        public async Task<IEnumerable<UserLawFirm>> GetAllActiveAsync()
        {
            return await _dbSet
                .Where(x => x.Status == "Active")
                .Include(x => x.User)
                .Include(x => x.LawFirm)
                .ToListAsync();
        }
    }
}