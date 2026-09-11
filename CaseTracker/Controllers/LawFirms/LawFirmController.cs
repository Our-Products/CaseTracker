using CaseTrackerApplication.Exceptions;
using CaseTrackerApplication.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CaseTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LawFirmController : ControllerBase
    {
        private readonly ILawFirmService _lawFirmService;

        public LawFirmController(
            ILawFirmService lawFirmService)
        {
            _lawFirmService = lawFirmService;
        }

        /// <summary>
        /// Get all law firms.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result =
                await _lawFirmService.GetAllAsync();

            return Ok(result);
        }

        /// <summary>
        /// Get all active law firms.
        /// </summary>
        [HttpGet("active")]
        public async Task<IActionResult> GetAllActive()
        {
            var result =
                await _lawFirmService.GetAllActiveAsync();

            return Ok(result);
        }

        /// <summary>
        /// Get law firm by ID.
        /// </summary>
        [HttpGet("{lawFirmId:guid}")]
        public async Task<IActionResult> GetById(
            Guid lawFirmId)
        {
            var result =
                await _lawFirmService.GetByIdAsync(
                    lawFirmId);

            if (result == null)
                throw new NotFoundException(
                    "Law firm not found.");

            return Ok(result);
        }

        /// <summary>
        /// Get law firm by registration number.
        /// </summary>
        [HttpGet("registration/{registrationNumber}")]
        public async Task<IActionResult>
            GetByRegistrationNumber(
                string registrationNumber)
        {
            if (string.IsNullOrWhiteSpace(
                registrationNumber))
            {
                throw new ValidationException(
                    "Registration number is required.");
            }

            var result =
                await _lawFirmService
                    .GetByRegistrationNumberAsync(
                        registrationNumber);

            if (result == null)
                throw new NotFoundException(
                    "Law firm not found.");

            return Ok(result);
        }

        /// <summary>
        /// Search law firms by name.
        /// </summary>
        [HttpGet("search")]
        public async Task<IActionResult> SearchByName(
            [FromQuery] string firmName)
        {
            if (string.IsNullOrWhiteSpace(firmName))
                throw new ValidationException(
                    "Firm name is required.");

            var result =
                await _lawFirmService
                    .SearchByNameAsync(firmName);

            return Ok(result);
        }
    }
}