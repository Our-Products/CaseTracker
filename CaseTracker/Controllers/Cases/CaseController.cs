using System;
using System.Security.Claims;
using System.Threading.Tasks;
using CaseTracker.Constants;
using CaseTracker.Extensions;
using CaseTrackerApplication.DTOs.Cases;
using CaseTrackerApplication.DTOs.Common;
using CaseTrackerApplication.Interfaces.Services.Cases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CaseTracker.Controllers.Cases
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CaseController : ControllerBase
    {
        private readonly ICaseService _caseService;

        public CaseController(ICaseService caseService)
        {
            _caseService = caseService;
        }

        /// <summary>
        /// Retrieves all cases for the current authenticated scope.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            // Note: In firm-scoped tenancy, pass firm ID when user is not superadmin
            var cases = await _caseService.GetAllCasesAsync(null);
            return Ok(ApiResponse<System.Collections.Generic.IEnumerable<CaseDto>>.SuccessResponse(cases));
        }

        /// <summary>
        /// Retrieves full details for a case by ID.
        /// </summary>
        [HttpGet("{caseId:guid}")]
        public async Task<IActionResult> GetById(Guid caseId)
        {
            var result = await _caseService.GetCaseByIdAsync(caseId);
            return Ok(ApiResponse<CaseDetailDto>.SuccessResponse(result));
        }

        /// <summary>
        /// Retrieves a case by its 16-character eCourts CNR Number.
        /// </summary>
        [HttpGet("cnr/{cnrNumber}")]
        public async Task<IActionResult> GetByCnr(string cnrNumber)
        {
            var result = await _caseService.GetCaseByCnrAsync(cnrNumber);
            return Ok(ApiResponse<CaseDto>.SuccessResponse(result));
        }

        /// <summary>
        /// Registers a new legal case matter.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = AppRoles.AdminOrLawyer)]
        public async Task<IActionResult> Create([FromBody] CreateCaseRequest request)
        {
            var userId = User.GetUserId() ?? Guid.Empty;
            var result = await _caseService.CreateCaseAsync(request, userId, null);
            return CreatedAtAction(nameof(GetById), new { caseId = result.CaseId }, ApiResponse<CaseDto>.SuccessResponse(result, "Case registered successfully."));
        }

        /// <summary>
        /// Updates an existing case.
        /// </summary>
        [HttpPut("{caseId:guid}")]
        [Authorize(Roles = AppRoles.AdminOrLawyer)]
        public async Task<IActionResult> Update(Guid caseId, [FromBody] UpdateCaseRequest request)
        {
            var userId = User.GetUserId() ?? Guid.Empty;
            var result = await _caseService.UpdateCaseAsync(caseId, request, userId);
            return Ok(ApiResponse<CaseDto>.SuccessResponse(result, "Case updated successfully."));
        }

        /// <summary>
        /// Archives / soft-deletes a case.
        /// </summary>
        [HttpDelete("{caseId:guid}")]
        [Authorize(Roles = AppRoles.SuperAdminOrAdmin)]
        public async Task<IActionResult> Delete(Guid caseId)
        {
            var userId = User.GetUserId() ?? Guid.Empty;
            await _caseService.DeleteCaseAsync(caseId, userId);
            return Ok(ApiResponse<string>.SuccessResponse("Case archived successfully."));
        }

        /// <summary>
        /// Triggers an explicit, authorized on-demand eCourts synchronization for a single case.
        /// </summary>
        [HttpPost("{caseId:guid}/sync")]
        [Authorize(Roles = AppRoles.AdminOrLawyer)]
        public async Task<IActionResult> SyncCase(Guid caseId)
        {
            var userId = User.GetUserId() ?? Guid.Empty;
            var result = await _caseService.SyncCaseNowAsync(caseId, userId);
            return Ok(ApiResponse<CaseDto>.SuccessResponse(result, "Case synchronization completed."));
        }

        /// <summary>
        /// Retrieves lawyers assigned to the specified case.
        /// </summary>
        [HttpGet("{caseId:guid}/lawyers")]
        public async Task<IActionResult> GetLawyers(Guid caseId)
        {
            var result = await _caseService.GetCaseLawyersAsync(caseId);
            return Ok(ApiResponse<System.Collections.Generic.IEnumerable<CaseLawyerItemDto>>.SuccessResponse(result));
        }

        /// <summary>
        /// Assigns an advocate to a case matter.
        /// </summary>
        [HttpPost("{caseId:guid}/lawyers")]
        [Authorize(Roles = AppRoles.AdminOrLawyer)]
        public async Task<IActionResult> AssignLawyer(Guid caseId, [FromBody] AssignCaseLawyerRequest request)
        {
            var userId = User.GetUserId() ?? Guid.Empty;
            var result = await _caseService.AssignLawyerAsync(caseId, request, userId);
            return Ok(ApiResponse<CaseLawyerItemDto>.SuccessResponse(result, "Lawyer assigned to case successfully."));
        }

        /// <summary>
        /// Removes an assigned advocate from a case matter.
        /// </summary>
        [HttpDelete("{caseId:guid}/lawyers/{lawyerId:guid}")]
        [Authorize(Roles = AppRoles.AdminOrLawyer)]
        public async Task<IActionResult> RemoveLawyer(Guid caseId, Guid lawyerId)
        {
            var userId = User.GetUserId() ?? Guid.Empty;
            await _caseService.RemoveLawyerAsync(caseId, lawyerId, userId);
            return Ok(ApiResponse<string>.SuccessResponse("Lawyer unassigned from case successfully."));
        }
    }
}
