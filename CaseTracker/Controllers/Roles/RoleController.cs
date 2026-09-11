using CaseTracker.Constants;
using CaseTrackerApplication.DTOs.Roles;
using CaseTrackerApplication.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CaseTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        /// <summary>
        /// Get all roles (Company SuperAdmin or Firm Admin).
        /// </summary>
        [Authorize(Roles = AppRoles.SuperAdminOrAdmin)]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var roles = await _roleService.GetAllAsync();

            return Ok(roles);
        }

        /// <summary>
        /// Get all active roles (Any authenticated user).
        /// </summary>
        [HttpGet("active")]
        public async Task<IActionResult> GetAllActive()
        {
            var roles = await _roleService.GetAllActiveAsync();

            return Ok(roles);
        }

        /// <summary>
        /// Get role by ID (Company SuperAdmin or Firm Admin).
        /// </summary>
        [Authorize(Roles = AppRoles.SuperAdminOrAdmin)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var role = await _roleService.GetByIdAsync(id);

            return Ok(role);
        }

        /// <summary>
        /// Get role by name (Company SuperAdmin or Firm Admin).
        /// </summary>
        [Authorize(Roles = AppRoles.SuperAdminOrAdmin)]
        [HttpGet("name/{roleName}")]
        public async Task<IActionResult> GetByName(string roleName)
        {
            var role = await _roleService.GetByNameAsync(roleName);

            return Ok(role);
        }

        /// <summary>
        /// Create a new role (Company SuperAdmin only).
        /// </summary>
        [Authorize(Roles = AppRoles.SuperAdmin)]
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateRoleRequest request)
        {
            var role = await _roleService.CreateAsync(request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = role.RoleId },
                role);
        }

        /// <summary>
        /// Update an existing role (Company SuperAdmin only).
        /// </summary>
        [Authorize(Roles = AppRoles.SuperAdmin)]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            string id,
            [FromBody] UpdateRoleRequest request)
        {
            var role = await _roleService.UpdateAsync(id, request);

            return Ok(role);
        }

        /// <summary>
        /// Delete a role (Company SuperAdmin only).
        /// </summary>
        [Authorize(Roles = AppRoles.SuperAdmin)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _roleService.DeleteAsync(id);

            return NoContent();
        }
    }
}