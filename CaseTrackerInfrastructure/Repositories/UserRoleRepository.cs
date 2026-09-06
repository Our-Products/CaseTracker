using CaseTrackerApplication.Interfaces;
using CaseTrackerDomain.Models;
using CaseTrackerInfrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CaseTrackerInfrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for UserRole junction entity.
    /// Encapsulates all database access logic for user-role assignments.
    /// </summary>
    public class UserRoleRepository : IUserRoleRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRoleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all roles assigned to a specific user asynchronously.
        /// </summary>
        public async Task<IEnumerable<UserRole>> GetByUserIdAsync(Guid userId)
        {
            return await _context.UserRoles
                .Where(x => x.UserId == userId)
                .Include(x => x.User)
                .Include(x => x.Role)
                .ToListAsync();
        }

        /// <summary>
        /// Get all users assigned to a specific role asynchronously.
        /// </summary>
        public async Task<IEnumerable<UserRole>> GetByRoleIdAsync(string roleId)
        {
            return await _context.UserRoles
                .Where(x => x.RoleId == roleId)
                .Include(x => x.User)
                .Include(x => x.Role)
                .ToListAsync();
        }

        /// <summary>
        /// Get a specific UserRole assignment asynchronously.
        /// </summary>
        public async Task<UserRole?> GetByUserAndRoleAsync(Guid userId, string roleId)
        {
            return await _context.UserRoles
                .Include(x => x.User)
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x => x.UserId == userId && x.RoleId == roleId);
        }

        /// <summary>
        /// Check if a user has a specific role asynchronously.
        /// </summary>
        public async Task<bool> UserHasRoleAsync(Guid userId, string roleId)
        {
            return await _context.UserRoles
                .AnyAsync(x => x.UserId == userId && x.RoleId == roleId);
        }

        /// <summary>
        /// Remove a specific role from a user asynchronously.
        /// </summary>
        public async Task RemoveUserRoleAsync(Guid userId, string roleId)
        {
            var userRole = await GetByUserAndRoleAsync(userId, roleId);
            if (userRole != null)
            {
                _context.UserRoles.Remove(userRole);
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Add a new UserRole assignment asynchronously.
        /// </summary>
        public async Task<UserRole> AddAsync(UserRole userRole)
        {
            await _context.UserRoles.AddAsync(userRole);
            return await Task.FromResult(userRole);
        }

        /// <summary>
        /// Save all pending changes to the database asynchronously.
        /// </summary>
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
