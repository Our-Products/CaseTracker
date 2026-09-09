using CaseTrackerApplication.Interfaces.Repositories;
using CaseTrackerDomain.Models;
using CaseTrackerInfrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CaseTrackerInfrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for Role entity.
    /// Encapsulates all database access logic for role management.
    /// </summary>
    public class RoleRepository : Repository<Role>, IRoleRepository
    {
        private readonly ApplicationDbContext _context;

        public RoleRepository(ApplicationDbContext context)
            : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Get Role by role name asynchronously.
        /// </summary>
        public async Task<Role?> GetByNameAsync(string roleName)
        {
            return await _dbSet
                .FirstOrDefaultAsync(x => x.RoleName == roleName);
        }

        /// <summary>
        /// Get all active roles asynchronously.
        /// </summary>
        public async Task<IEnumerable<Role>> GetAllActiveAsync()
        {
            return await _dbSet
                .Where(x => x.Status == "Active")
                .OrderBy(x => x.RoleName)
                .ToListAsync();
        }

        /// <summary>
        /// Check if a role name already exists asynchronously.
        /// </summary>
        public async Task<bool> RoleNameExistsAsync(string roleName)
        {
            return await _dbSet
                .AnyAsync(x => x.RoleName == roleName);
        }
    }
}