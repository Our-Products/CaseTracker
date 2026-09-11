using CaseTrackerApplication.DTOs.UserRoles;
using CaseTrackerDomain.Models;

namespace CaseTrackerApplication.Interfaces.Services
{
    public interface IUserRoleService
    {
        Task<IEnumerable<UserRole>> GetAllAsync();

        Task<IEnumerable<UserRole>> GetAllActiveAsync();

        Task<IEnumerable<UserRole>> GetByUserIdAsync(
            Guid userId);

        Task<IEnumerable<UserRole>> GetByRoleIdAsync(
            string roleId);

        Task<UserRole?> GetByUserAndRoleAsync(
            Guid userId,
            string roleId);

        Task<IEnumerable<string>> GetRoleNamesByUserIdAsync(
            Guid userId);

        Task<bool> IsUserAssignedToRoleAsync(
            Guid userId,
            string roleId);

        Task<UserRole> AssignRoleAsync(
            AssignUserRoleRequest request);

        Task RemoveRoleAsync(
            Guid userId,
            string roleId);
    }
}