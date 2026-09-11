using CaseTrackerApplication.DTOs.UserRoles;
using CaseTrackerApplication.Exceptions;
using CaseTrackerApplication.Interfaces.Repositories;
using CaseTrackerApplication.Interfaces.Services;
using CaseTrackerDomain.Models;

namespace CaseTrackerApplication.Services
{
    public class UserRoleService : IUserRoleService
    {
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UserRoleService(
            IUserRoleRepository userRoleRepository,
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IUnitOfWork unitOfWork)
        {
            _userRoleRepository = userRoleRepository;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<UserRole>> GetAllAsync()
        {
            return await _userRoleRepository.GetAllAsync();
        }

        public async Task<IEnumerable<UserRole>> GetAllActiveAsync()
        {
            return await _userRoleRepository.GetAllActiveAsync();
        }

        public async Task<IEnumerable<UserRole>> GetByUserIdAsync(
            Guid userId)
        {
            return await _userRoleRepository
                .GetByUserIdAsync(userId);
        }

        public async Task<IEnumerable<UserRole>> GetByRoleIdAsync(
            string roleId)
        {
            return await _userRoleRepository
                .GetByRoleIdAsync(roleId);
        }

        public async Task<UserRole?> GetByUserAndRoleAsync(
            Guid userId,
            string roleId)
        {
            return await _userRoleRepository
                .GetByUserAndRoleAsync(userId, roleId);
        }

        public async Task<IEnumerable<string>>
            GetRoleNamesByUserIdAsync(Guid userId)
        {
            return await _userRoleRepository
                .GetRoleNamesByUserIdAsync(userId);
        }

        public async Task<bool> IsUserAssignedToRoleAsync(
            Guid userId,
            string roleId)
        {
            return await _userRoleRepository
                .IsUserAssignedToRoleAsync(userId, roleId);
        }

        public async Task<UserRole> AssignRoleAsync(
            AssignUserRoleRequest request)
        {
            if (request == null)
                throw new ValidationException(
                    "Request is required.");

            if (request.UserId == Guid.Empty)
                throw new ValidationException(
                    "User ID is required.");

            if (string.IsNullOrWhiteSpace(request.RoleId))
                throw new ValidationException(
                    "Role ID is required.");

            var user = await _userRepository
                .GetByIdAsync(request.UserId);

            if (user == null)
                throw new NotFoundException(
                    "User not found.");

            if (user.Status != "Active")
                throw new ConflictException(
                    "User is inactive.");

            var role = await _roleRepository
                .GetByIdAsync(request.RoleId);

            if (role == null)
                throw new NotFoundException(
                    "Role not found.");

            if (role.Status != "Active")
                throw new ConflictException(
                    "Role is inactive.");

            var exists = await _userRoleRepository
                .IsUserAssignedToRoleAsync(
                    request.UserId,
                    request.RoleId);

            if (exists)
                throw new ConflictException(
                    "User is already assigned to this role.");

            var now = DateTimeOffset.UtcNow;

            var userRole = new UserRole
            {
                UserId = request.UserId,
                RoleId = request.RoleId,
                CreatedAt = now,
                UpdatedAt = now,
                CreatedBy = request.UserId,
                UpdatedBy = request.UserId
            };

            await _userRoleRepository.AddAsync(userRole);

            await _unitOfWork.SaveChangesAsync();

            return userRole;
        }

        public async Task RemoveRoleAsync(
            Guid userId,
            string roleId)
        {
            var userRole = await _userRoleRepository
                .GetByUserAndRoleAsync(userId, roleId);

            if (userRole == null)
                throw new NotFoundException(
                    "User-role assignment not found.");

            await _userRoleRepository.DeleteAsync(userRole);

            await _unitOfWork.SaveChangesAsync();
        }
    }
}