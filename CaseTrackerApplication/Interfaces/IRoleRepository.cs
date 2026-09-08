using CaseTrackerDomain.Models;

namespace CaseTrackerApplication.Interfaces
{
    /// <summary>
    /// Repository interface for Role entity operations.
    /// Defines all data access contracts for role management.
    /// </summary>
    public interface IRoleRepository
    {
        /// <summary>
        /// Get Role by role name asynchronously.
        /// </summary>
        Task<Role?> GetByNameAsync(string roleName);

        /// <summary>
        /// Get all active roles asynchronously.
        /// </summary>
        Task<IEnumerable<Role>> GetAllActiveAsync();

        /// <summary>
        /// Check if a role name already exists asynchronously.
        /// </summary>
        Task<bool> RoleNameExistsAsync(string roleName);
    }
}
