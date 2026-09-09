using CaseTrackerDomain.Models;

namespace CaseTrackerApplication.Interfaces
{
    /// <summary>
    /// Repository interface for UserRole junction entity operations.
    /// Defines all data access contracts for user-role assignments.
    /// </summary>
    public interface IUserRoleRepository
    {
        /// <summary>
        /// Get all roles assigned to a specific user asynchronously.
        /// </summary>
        Task<IEnumerable<UserRole>> GetByUserIdAsync(Guid userId);

        /// <summary>
        /// Get all users assigned to a specific role asynchronously.
        /// </summary>
        Task<IEnumerable<UserRole>> GetByRoleIdAsync(string roleId);

        /// <summary>
        /// Get a specific UserRole assignment asynchronously.
        /// </summary>
        Task<UserRole?> GetByUserAndRoleAsync(Guid userId, string roleId);

        /// <summary>
        /// Check if a user has a specific role asynchronously.
        /// </summary>
        Task<bool> UserHasRoleAsync(Guid userId, string roleId);

        /// <summary>
        /// Remove a specific role from a user asynchronously.
        /// </summary>
        Task RemoveUserRoleAsync(Guid userId, string roleId);

        /// <summary>
        /// Add a new UserRole assignment asynchronously.
        /// </summary>
        Task<UserRole> AddAsync(UserRole userRole);

        /// <summary>
        /// Save all pending changes to the database asynchronously.
        /// </summary>
        Task SaveChangesAsync();
    }
}
