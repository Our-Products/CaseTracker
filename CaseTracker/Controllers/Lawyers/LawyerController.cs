using CaseTracker.Constants;
using CaseTracker.Extensions;
using CaseTrackerApplication.Exceptions;
using CaseTrackerApplication.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CaseTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LawyerController : ControllerBase
    {
        private readonly ILawyerService _lawyerService;

        public LawyerController(
            ILawyerService lawyerService)
        {
            _lawyerService = lawyerService;
        }

        /// <summary>
        /// Get all lawyers (Company SuperAdmin or Firm Admin).
        /// </summary>
        [Authorize(Roles = AppRoles.SuperAdminOrAdmin)]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result =
                await _lawyerService.GetAllAsync();

            return Ok(result);
        }

        /// <summary>
        /// Get all active lawyers (Authenticated users).
        /// </summary>
        [HttpGet("active")]
        public async Task<IActionResult> GetAllActive()
        {
            var result =
                await _lawyerService.GetAllActiveAsync();

            return Ok(result);
        }

        /// <summary>
        /// Get lawyer by ID (Authenticated users).
        /// </summary>
        [HttpGet("{lawyerId:guid}")]
        public async Task<IActionResult> GetById(
            Guid lawyerId)
        {
            var result =
                await _lawyerService.GetByIdAsync(
                    lawyerId);

            if (result == null)
                throw new NotFoundException(
                    "Lawyer not found.");

            return Ok(result);
        }

        /// <summary>
        /// Get lawyer by User ID (Admin or Self).
        /// </summary>
        [HttpGet("user/{userId:guid}")]
        public async Task<IActionResult> GetByUserId(
            Guid userId)
        {
            if (!User.CanAccessUser(userId))
            {
                return Forbid();
            }

            var result =
                await _lawyerService.GetByUserIdAsync(
                    userId);

            if (result == null)
                throw new NotFoundException(
                    "Lawyer not found.");

            return Ok(result);
        }

        /// <summary>
        /// Get lawyer by Bar Council ID (Admin or Lawyer).
        /// </summary>
        [Authorize(Roles = AppRoles.AdminOrLawyer)]
        [HttpGet("bar-council/{barCouncilId}")]
        public async Task<IActionResult> GetByBarCouncilId(
            string barCouncilId)
        {
            var result =
                await _lawyerService
                    .GetByBarCouncilIdAsync(
                        barCouncilId);

            if (result == null)
                throw new NotFoundException(
                    "Lawyer not found.");

            return Ok(result);
        }

        /// <summary>
        /// Get all lawyers belonging to a law firm.
        /// </summary>
        [HttpGet("lawfirm/{lawFirmId:guid}")]
        public async Task<IActionResult> GetByLawFirmId(
            Guid lawFirmId)
        {
            var result =
                await _lawyerService
                    .GetByLawFirmIdAsync(
                        lawFirmId);

            return Ok(result);
        }

        /// <summary>
        /// Search lawyers by full name.
        /// </summary>
        [HttpGet("search")]
        public async Task<IActionResult> SearchByName(
            [FromQuery] string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                throw new ValidationException(
                    "Search name is required.");

            var result =
                await _lawyerService
                    .SearchByNameAsync(fullName);

            return Ok(result);
        }
    }
}