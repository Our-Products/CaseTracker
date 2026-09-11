using CaseTracker.Constants;
using CaseTracker.Extensions;
using CaseTrackerApplication.DTOs.UserLawFirms;
using CaseTrackerApplication.Exceptions;
using CaseTrackerApplication.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CaseTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserLawFirmController : ControllerBase
    {
        private readonly IUserLawFirmService _userLawFirmService;

        public UserLawFirmController(
            IUserLawFirmService userLawFirmService)
        {
            _userLawFirmService = userLawFirmService;
        }

        [Authorize(Roles = AppRoles.SuperAdmin)]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result =
                await _userLawFirmService.GetAllAsync();

            return Ok(result);
        }

        [Authorize(Roles = AppRoles.SuperAdmin)]
        [HttpGet("active")]
        public async Task<IActionResult> GetAllActive()
        {
            var result =
                await _userLawFirmService.GetAllActiveAsync();

            return Ok(result);
        }

        [HttpGet("user/{userId:guid}")]
        public async Task<IActionResult> GetByUserId(
            Guid userId)
        {
            if (!User.CanAccessUser(userId))
            {
                return Forbid();
            }

            var result =
                await _userLawFirmService
                    .GetByUserIdAsync(userId);

            return Ok(result);
        }

        [Authorize(Roles = AppRoles.AdminOrLawyer)]
        [HttpGet("lawfirm/{lawFirmId:guid}")]
        public async Task<IActionResult> GetByLawFirmId(
            Guid lawFirmId)
        {
            var result =
                await _userLawFirmService
                    .GetByLawFirmIdAsync(lawFirmId);

            return Ok(result);
        }

        [HttpGet(
            "user/{userId:guid}/lawfirm/{lawFirmId:guid}")]
        public async Task<IActionResult> GetByUserAndLawFirm(
            Guid userId,
            Guid lawFirmId)
        {
            if (!User.CanAccessUser(userId))
            {
                return Forbid();
            }

            var result =
                await _userLawFirmService
                    .GetByUserAndLawFirmAsync(
                        userId,
                        lawFirmId);

            if (result == null)
                throw new NotFoundException(
                    "User is not a member of this law firm.");

            return Ok(result);
        }

        [HttpGet(
            "user/{userId:guid}/lawfirm/{lawFirmId:guid}/exists")]
        public async Task<IActionResult> IsUserInLawFirm(
            Guid userId,
            Guid lawFirmId)
        {
            if (!User.CanAccessUser(userId))
            {
                return Forbid();
            }

            var exists =
                await _userLawFirmService
                    .IsUserInLawFirmAsync(
                        userId,
                        lawFirmId);

            return Ok(new { exists });
        }

        [Authorize(Roles = AppRoles.AdminOrLawyer)]
        [HttpPost]
        public async Task<IActionResult> AddUserToLawFirm(
            [FromBody] AddUserLawFirmRequest request)
        {
            var result =
                await _userLawFirmService
                    .AddUserToLawFirmAsync(request);

            return Ok(result);
        }

        [Authorize(Roles = AppRoles.AdminOrLawyer)]
        [HttpDelete(
            "user/{userId:guid}/lawfirm/{lawFirmId:guid}")]
        public async Task<IActionResult> RemoveUserFromLawFirm(
            Guid userId,
            Guid lawFirmId)
        {
            await _userLawFirmService
                .RemoveUserFromLawFirmAsync(
                    userId,
                    lawFirmId);

            return NoContent();
        }
    }
}