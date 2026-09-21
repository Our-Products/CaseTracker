using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CaseTracker.Constants;
using CaseTracker.Extensions;
using CaseTrackerApplication.DTOs.Common;
using CaseTrackerApplication.DTOs.Hearings;
using CaseTrackerApplication.Interfaces.Services.Hearings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CaseTracker.Controllers.Hearings
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class HearingController : ControllerBase
    {
        private readonly IHearingService _hearingService;

        public HearingController(IHearingService hearingService)
        {
            _hearingService = hearingService;
        }

        /// <summary>
        /// Retrieves the daily cause board for a specific date.
        /// </summary>
        [HttpGet("daily-board")]
        public async Task<IActionResult> GetDailyBoard([FromQuery] DateTime? date, [FromQuery] Guid? courtId)
        {
            var targetDate = date ?? DateTime.Today;
            var list = await _hearingService.GetDailyBoardAsync(targetDate, null, courtId);
            return Ok(ApiResponse<IEnumerable<HearingDto>>.SuccessResponse(list));
        }

        /// <summary>
        /// Retrieves all hearings for a case.
        /// </summary>
        [HttpGet("case/{caseId:guid}")]
        public async Task<IActionResult> GetByCase(Guid caseId)
        {
            var list = await _hearingService.GetHearingsByCaseAsync(caseId);
            return Ok(ApiResponse<IEnumerable<HearingDto>>.SuccessResponse(list));
        }

        /// <summary>
        /// Retrieves a single hearing by ID.
        /// </summary>
        [HttpGet("{hearingId:guid}")]
        public async Task<IActionResult> GetById(Guid hearingId)
        {
            var result = await _hearingService.GetHearingByIdAsync(hearingId);
            return Ok(ApiResponse<HearingDto>.SuccessResponse(result));
        }

        /// <summary>
        /// Creates a new hearing schedule entry.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = AppRoles.AdminOrLawyer)]
        public async Task<IActionResult> Create([FromBody] CreateHearingRequest request)
        {
            var userId = User.GetUserId() ?? Guid.Empty;
            var result = await _hearingService.CreateHearingAsync(request, userId);
            return CreatedAtAction(nameof(GetById), new { hearingId = result.HearingId }, ApiResponse<HearingDto>.SuccessResponse(result, "Hearing logged successfully."));
        }

        /// <summary>
        /// Updates a hearing and records daily business/proceedings.
        /// </summary>
        [HttpPut("{hearingId:guid}")]
        [Authorize(Roles = AppRoles.AdminOrLawyer)]
        public async Task<IActionResult> Update(Guid hearingId, [FromBody] UpdateHearingRequest request)
        {
            var userId = User.GetUserId() ?? Guid.Empty;
            var result = await _hearingService.UpdateHearingAsync(hearingId, request, userId);
            return Ok(ApiResponse<HearingDto>.SuccessResponse(result, "Hearing updated successfully."));
        }

        /// <summary>
        /// Deletes a hearing schedule record.
        /// </summary>
        [HttpDelete("{hearingId:guid}")]
        [Authorize(Roles = AppRoles.AdminOrLawyer)]
        public async Task<IActionResult> Delete(Guid hearingId)
        {
            var userId = User.GetUserId() ?? Guid.Empty;
            await _hearingService.DeleteHearingAsync(hearingId, userId);
            return Ok(ApiResponse<string>.SuccessResponse("Hearing removed successfully."));
        }
    }
}
