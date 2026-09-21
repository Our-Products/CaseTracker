using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CaseTracker.Constants;
using CaseTracker.Extensions;
using CaseTrackerApplication.DTOs.Clients;
using CaseTrackerApplication.DTOs.Common;
using CaseTrackerApplication.Interfaces.Services.Clients;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CaseTracker.Controllers.Clients
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ClientController : ControllerBase
    {
        private readonly IClientService _clientService;

        public ClientController(IClientService clientService)
        {
            _clientService = clientService;
        }

        /// <summary>
        /// Retrieves or searches clients.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? search)
        {
            var clients = await _clientService.GetClientsAsync(search, null);
            return Ok(ApiResponse<IEnumerable<ClientDto>>.SuccessResponse(clients));
        }

        /// <summary>
        /// Retrieves a client by ID.
        /// </summary>
        [HttpGet("{clientId:guid}")]
        public async Task<IActionResult> GetById(Guid clientId)
        {
            var result = await _clientService.GetClientByIdAsync(clientId);
            return Ok(ApiResponse<ClientDto>.SuccessResponse(result));
        }

        /// <summary>
        /// Creates a new client profile.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = AppRoles.AdminOrLawyer)]
        public async Task<IActionResult> Create([FromBody] CreateClientRequest request)
        {
            var userId = User.GetUserId() ?? Guid.Empty;
            var result = await _clientService.CreateClientAsync(request, userId, null);
            return CreatedAtAction(nameof(GetById), new { clientId = result.ClientId }, ApiResponse<ClientDto>.SuccessResponse(result, "Client created successfully."));
        }

        /// <summary>
        /// Updates an existing client profile.
        /// </summary>
        [HttpPut("{clientId:guid}")]
        [Authorize(Roles = AppRoles.AdminOrLawyer)]
        public async Task<IActionResult> Update(Guid clientId, [FromBody] UpdateClientRequest request)
        {
            var userId = User.GetUserId() ?? Guid.Empty;
            var result = await _clientService.UpdateClientAsync(clientId, request, userId);
            return Ok(ApiResponse<ClientDto>.SuccessResponse(result, "Client updated successfully."));
        }

        /// <summary>
        /// Archives / soft-deletes a client.
        /// </summary>
        [HttpDelete("{clientId:guid}")]
        [Authorize(Roles = AppRoles.SuperAdminOrAdmin)]
        public async Task<IActionResult> Delete(Guid clientId)
        {
            var userId = User.GetUserId() ?? Guid.Empty;
            await _clientService.DeleteClientAsync(clientId, userId);
            return Ok(ApiResponse<string>.SuccessResponse("Client archived successfully."));
        }

        /// <summary>
        /// Links a client to a case as a represented party (Petitioner, Respondent, etc.).
        /// </summary>
        [HttpPost("link-case/{caseId:guid}")]
        [Authorize(Roles = AppRoles.AdminOrLawyer)]
        public async Task<IActionResult> LinkClientToCase(Guid caseId, [FromBody] LinkClientToCaseRequest request)
        {
            var userId = User.GetUserId() ?? Guid.Empty;
            await _clientService.LinkClientToCaseAsync(caseId, request, userId);
            return Ok(ApiResponse<string>.SuccessResponse("Client linked to case successfully."));
        }

        /// <summary>
        /// Removes client representation from a case.
        /// </summary>
        [HttpDelete("link-case/{caseId:guid}/{clientId:guid}")]
        [Authorize(Roles = AppRoles.AdminOrLawyer)]
        public async Task<IActionResult> UnlinkClientFromCase(Guid caseId, Guid clientId)
        {
            var userId = User.GetUserId() ?? Guid.Empty;
            await _clientService.UnlinkClientFromCaseAsync(caseId, clientId, userId);
            return Ok(ApiResponse<string>.SuccessResponse("Client representation removed from case."));
        }
    }
}
