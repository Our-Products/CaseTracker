using CaseTrackerApplication.Interfaces;
using CaseTrackerDomain.Models;
using CaseTrackerInfrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CaseTrackerInfrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for Role entity.
    /// Encapsulates all database access logic for role management.
    /// </summary>
    public class RoleRepository : IRoleRepository
    {
        private readonly ApplicationDbContext _context;

        public RoleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get Role by role name asynchronously.
        /// </summary>
        public async Task<Role?> GetByNameAsync(string roleName)
        {
            return await _context.Roles
                .FirstOrDefaultAsync(x => x.RoleName == roleName);
        }

        /// <summary>
        /// Get all active roles asynchronously.
        /// </summary>
        public async Task<IEnumerable<Role>> GetAllActiveAsync()
        {
            return await _context.Roles
                .Where(x => x.Status == "Active")
                .OrderBy(x => x.RoleName)
                .ToListAsync();
        }

        /// <summary>
        /// Check if a role name already exists asynchronously.
        /// </summary>
        public async Task<bool> RoleNameExistsAsync(string roleName)
        {
            return await _context.Roles
                .AnyAsync(x => x.RoleName == roleName);
        }
    }
}
