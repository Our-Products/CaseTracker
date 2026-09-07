using CaseTrackerApplication.DTOs.UserRoles;
using CaseTrackerApplication.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CaseTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserRoleController : ControllerBase
    {
        private readonly IUserRoleService _userRoleService;

        public UserRoleController(
            IUserRoleService userRoleService)
        {
            _userRoleService = userRoleService;
        }

        /// <summary>
        /// Get all user-role assignments.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userRoles =
                await _userRoleService.GetAllAsync();

            return Ok(userRoles);
        }

        /// <summary>
        /// Get all active user-role assignments.
        /// </summary>
        [HttpGet("active")]
        public async Task<IActionResult> GetAllActive()
        {
            var userRoles =
                await _userRoleService.GetAllActiveAsync();

            return Ok(userRoles);
        }

        /// <summary>
        /// Get all roles assigned to a user.
        /// </summary>
        [HttpGet("user/{userId:guid}")]
        public async Task<IActionResult> GetByUserId(
            Guid userId)
        {
            var userRoles =
                await _userRoleService.GetByUserIdAsync(userId);

            return Ok(userRoles);
        }

        /// <summary>
        /// Get all users assigned to a role.
        /// </summary>
        [HttpGet("role/{roleId}")]
        public async Task<IActionResult> GetByRoleId(
            string roleId)
        {
            var userRoles =
                await _userRoleService.GetByRoleIdAsync(roleId);

            return Ok(userRoles);
        }

        /// <summary>
        /// Get a specific user-role assignment.
        /// </summary>
        [HttpGet("user/{userId:guid}/role/{roleId}")]
        public async Task<IActionResult> GetByUserAndRole(
            Guid userId,
            string roleId)
        {
            var userRole =
                await _userRoleService
                    .GetByUserAndRoleAsync(
                        userId,
                        roleId);

            return Ok(userRole);
        }

        /// <summary>
        /// Get role names assigned to a user.
        /// </summary>
        [HttpGet("user/{userId:guid}/role-names")]
        public async Task<IActionResult> GetRoleNamesByUserId(
            Guid userId)
        {
            var roleNames =
                await _userRoleService
                    .GetRoleNamesByUserIdAsync(userId);

            return Ok(roleNames);
        }

        /// <summary>
        /// Check whether a user is assigned to a role.
        /// </summary>
        [HttpGet("user/{userId:guid}/role/{roleId}/exists")]
        public async Task<IActionResult> IsUserAssignedToRole(
            Guid userId,
            string roleId)
        {
            var exists =
                await _userRoleService
                    .IsUserAssignedToRoleAsync(
                        userId,
                        roleId);

            return Ok(new { exists });
        }

        /// <summary>
        /// Assign a role to a user.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AssignRole(
            [FromBody] AssignUserRoleRequest request)
        {
            var userRole =
                await _userRoleService.AssignRoleAsync(request);

            return Ok(userRole);
        }

        /// <summary>
        /// Remove a role from a user.
        /// </summary>
        [HttpDelete("user/{userId:guid}/role/{roleId}")]
        public async Task<IActionResult> RemoveRole(
            Guid userId,
            string roleId)
        {
            await _userRoleService.RemoveRoleAsync(
                userId,
                roleId);

            return NoContent();
        }
    }
}