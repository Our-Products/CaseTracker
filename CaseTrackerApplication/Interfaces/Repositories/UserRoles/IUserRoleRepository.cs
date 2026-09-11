using CaseTrackerDomain.Models;

namespace CaseTrackerApplication.Interfaces.Repositories
{
    /// <summary>
    /// Repository interface for UserRole entity operations.
    /// </summary>
    public interface IUserRoleRepository : IRepository<UserRole>
    {
        Task<IEnumerable<string>> GetRoleNamesByUserIdAsync(Guid userId);
        Task<IEnumerable<UserRole>> GetByUserIdAsync(Guid userId);

        Task<IEnumerable<UserRole>> GetByRoleIdAsync(string roleId);

        Task<UserRole?> GetByUserAndRoleAsync(
            Guid userId,
            string roleId);

        Task<bool> IsUserAssignedToRoleAsync(
            Guid userId,
            string roleId);

        Task<IEnumerable<UserRole>> GetAllActiveAsync();
    }
}