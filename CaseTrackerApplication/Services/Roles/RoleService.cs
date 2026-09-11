using CaseTrackerApplication.DTOs.Roles;
using CaseTrackerApplication.Exceptions;
using CaseTrackerApplication.Interfaces.Repositories;
using CaseTrackerApplication.Interfaces.Services;
using CaseTrackerDomain.Models;

namespace CaseTrackerApplication.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RoleService(
            IRoleRepository roleRepository,
            IUnitOfWork unitOfWork)
        {
            _roleRepository = roleRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Role>> GetAllAsync()
        {
            return await _roleRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Role>> GetAllActiveAsync()
        {
            return await _roleRepository.GetAllActiveAsync();
        }

        public async Task<Role?> GetByIdAsync(string roleId)
        {
            return await _roleRepository.GetByIdAsync(roleId);
        }

        public async Task<Role?> GetByNameAsync(string roleName)
        {
            return await _roleRepository.GetByNameAsync(roleName);
        }

        public async Task<bool> RoleNameExistsAsync(
            string roleName)
        {
            return await _roleRepository
                .RoleNameExistsAsync(roleName);
        }

        public async Task<Role> CreateAsync(
            CreateRoleRequest request)
        {
            if (request == null)
                throw new ValidationException(
                    "Request is required.");

            if (string.IsNullOrWhiteSpace(request.RoleId))
                throw new ValidationException(
                    "Role ID is required.");

            if (string.IsNullOrWhiteSpace(request.RoleName))
                throw new ValidationException(
                    "Role name is required.");

            var roleExists =
                await _roleRepository
                    .GetByIdAsync(request.RoleId);

            if (roleExists != null)
                throw new ConflictException(
                    "Role ID already exists.");

            var roleNameExists =
                await _roleRepository
                    .RoleNameExistsAsync(request.RoleName);

            if (roleNameExists)
                throw new ConflictException(
                    "Role name already exists.");

            var now = DateTimeOffset.UtcNow;

            var role = new Role
            {
                RoleId = request.RoleId,
                RoleName = request.RoleName,
                Status = "Active",
                CreatedAt = now,
                UpdatedAt = now
            };

            await _roleRepository.AddAsync(role);

            await _unitOfWork.SaveChangesAsync();

            return role;
        }

        public async Task<Role> UpdateAsync(
            string roleId,
            UpdateRoleRequest request)
        {
            if (string.IsNullOrWhiteSpace(roleId))
                throw new ValidationException(
                    "Role ID is required.");

            if (request == null)
                throw new ValidationException(
                    "Request is required.");

            if (string.IsNullOrWhiteSpace(request.RoleName))
                throw new ValidationException(
                    "Role name is required.");

            if (string.IsNullOrWhiteSpace(request.Status))
                throw new ValidationException(
                    "Role status is required.");

            var role =
                await _roleRepository.GetByIdAsync(roleId);

            if (role == null)
                throw new NotFoundException(
                    "Role not found.");

            var roleNameExists =
                await _roleRepository
                    .RoleNameExistsAsync(request.RoleName);

            if (roleNameExists &&
                !string.Equals(
                    role.RoleName,
                    request.RoleName,
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new ConflictException(
                    "Role name already exists.");
            }

            role.RoleName = request.RoleName;
            role.Status = request.Status;
            role.UpdatedAt = DateTimeOffset.UtcNow;

            await _roleRepository.UpdateAsync(role);

            await _unitOfWork.SaveChangesAsync();

            return role;
        }

        public async Task DeleteAsync(string roleId)
        {
            if (string.IsNullOrWhiteSpace(roleId))
                throw new ValidationException(
                    "Role ID is required.");

            var role =
                await _roleRepository.GetByIdAsync(roleId);

            if (role == null)
                throw new NotFoundException(
                    "Role not found.");

            await _roleRepository.DeleteAsync(role);

            await _unitOfWork.SaveChangesAsync();
        }
    }
}