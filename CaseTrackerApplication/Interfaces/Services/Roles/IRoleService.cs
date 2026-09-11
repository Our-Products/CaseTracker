using CaseTrackerApplication.DTOs.Roles;
using CaseTrackerDomain.Models;

namespace CaseTrackerApplication.Interfaces.Services
{
    public interface IRoleService
    {
        Task<IEnumerable<Role>> GetAllAsync();

        Task<IEnumerable<Role>> GetAllActiveAsync();

        Task<Role?> GetByIdAsync(string roleId);

        Task<Role?> GetByNameAsync(string roleName);

        Task<bool> RoleNameExistsAsync(string roleName);

        Task<Role> CreateAsync(CreateRoleRequest request);

        Task<Role> UpdateAsync(
            string roleId,
            UpdateRoleRequest request);

        Task DeleteAsync(string roleId);
    }
}