using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CaseTrackerApplication.DTOs.Common;
using CaseTrackerApplication.DTOs.Courts;
using CaseTrackerApplication.Interfaces.Services.Courts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CaseTracker.Controllers.Courts
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CourtController : ControllerBase
    {
        private readonly ICourtMasterService _courtMasterService;

        public CourtController(ICourtMasterService courtMasterService)
        {
            _courtMasterService = courtMasterService;
        }

        /// <summary>
        /// Retrieves all states from the local master database.
        /// </summary>
        [HttpGet("states")]
        public async Task<IActionResult> GetStates()
        {
            var states = await _courtMasterService.GetStatesAsync();
            return Ok(ApiResponse<IEnumerable<StateDto>>.SuccessResponse(states));
        }

        /// <summary>
        /// Retrieves all districts for a state from the local master database.
        /// </summary>
        [HttpGet("states/{stateId:guid}/districts")]
        public async Task<IActionResult> GetDistricts(Guid stateId)
        {
            var districts = await _courtMasterService.GetDistrictsAsync(stateId);
            return Ok(ApiResponse<IEnumerable<DistrictDto>>.SuccessResponse(districts));
        }

        /// <summary>
        /// Retrieves all court complexes for a district from the local master database.
        /// </summary>
        [HttpGet("districts/{districtId:guid}/complexes")]
        public async Task<IActionResult> GetComplexes(Guid districtId)
        {
            var complexes = await _courtMasterService.GetComplexesAsync(districtId);
            return Ok(ApiResponse<IEnumerable<CourtComplexDto>>.SuccessResponse(complexes));
        }

        /// <summary>
        /// Retrieves all individual courts for a court complex from the local master database.
        /// </summary>
        [HttpGet("complexes/{complexId:guid}/courts")]
        public async Task<IActionResult> GetCourts(Guid complexId)
        {
            var courts = await _courtMasterService.GetCourtsAsync(complexId);
            return Ok(ApiResponse<IEnumerable<CourtDto>>.SuccessResponse(courts));
        }
    }
}
