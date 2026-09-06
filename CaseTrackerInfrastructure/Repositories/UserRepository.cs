using CaseTrackerApplication.Interfaces.Repositories;
using CaseTrackerDomain.Models;
using CaseTrackerInfrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CaseTrackerInfrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for User-specific data access operations.
    /// Common CRUD operations are inherited from Repository<User>.
    /// </summary>
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context)
            : base(context)
        {
        }

        public async Task<User?> GetByMobileNumberAsync(string mobileNumber)
        {
            return await _dbSet
                .FirstOrDefaultAsync(x => x.MobileNumber == mobileNumber);
        }

        public async Task<bool> MobileNumberExistsAsync(string mobileNumber)
        {
            return await _dbSet
                .AnyAsync(x => x.MobileNumber == mobileNumber);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _dbSet
                .AnyAsync(x => x.Email == email);
        }
    }
}