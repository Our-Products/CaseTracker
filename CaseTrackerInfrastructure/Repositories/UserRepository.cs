using CaseTrackerApplication.Interfaces;
using CaseTrackerDomain.Models;
using CaseTrackerInfrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CaseTrackerInfrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for Lawyer entity.
    /// Encapsulates all database access logic for lawyers.
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get user by mobile number
        /// </summary>
        public async Task<User?> GetByMobileNumberAsync(string mobileNumber)
        {
            return await _context.Users
                .SingleOrDefaultAsync(x => x.MobileNumber == mobileNumber);
        }

        /// <summary>
        /// Get user by ID
        /// </summary>
        public async Task<User?> GetByIdAsync(Guid userId)
        {
            return await _context.Users
                .FirstOrDefaultAsync(x => x.UserId == userId);
        }

        /// <summary>
        /// Check if mobile number already exists
        /// </summary>
        public async Task<bool> MobileNumberExistsAsync(string mobileNumber)
        {
            return await _context.Users
                .AnyAsync(x => x.MobileNumber == mobileNumber);
        }

        /// <summary>
        /// Get all users
        /// </summary>
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users
                .AsNoTracking()
                .ToListAsync();
        }

        /// <summary>
        /// Add new user
        /// </summary>
        public async Task<User> AddAsync(User user)
        {
            _context.Users.Add(user);
            return await Task.FromResult(user);
        }

        /// <summary>
        /// Update existing user
        /// </summary>
        public async Task<User> UpdateAsync(User user)
        {
            _context.Entry(user).State = EntityState.Modified;
            return await Task.FromResult(user);
        }

        /// <summary>
        /// Delete user by ID
        /// </summary>
        public async Task DeleteAsync(Guid userId)
        {
            var user = await GetByIdAsync(userId);
            if (user != null)
            {
                _context.Users.Remove(user);
            }
        }

        /// <summary>
        /// Save all pending changes
        /// </summary>
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
