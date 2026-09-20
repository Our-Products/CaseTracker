using CaseTrackerApplication.DTOs.AI;
using CaseTrackerApplication.DTOs.Common;
using CaseTrackerApplication.AI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CaseTracker.Controllers.AI
{
    /// <summary>
    /// Exposes AI question-answer capabilities via the REST API.
    /// The controller depends only on <see cref="IAIService"/>; the
    /// concrete AI provider is wired up in the Infrastructure layer.
    /// </summary>
    [ApiController]
    [Route("api/ai")]
    [Authorize]
    public class AIController : ControllerBase
    {
        private readonly IAIService _aiService;

        public AIController(IAIService aiService)
        {
            _aiService = aiService;
        }

        /// <summary>
        /// Ask the AI a question and receive a plain-text answer.
        /// </summary>
        /// <param name="request">The prompt to send to the AI provider.</param>
        /// <param name="cancellationToken">
        /// Automatically provided by ASP.NET Core; propagates client disconnect.
        /// </param>
        /// <returns>
        /// <see cref="AskResponse"/> wrapped in the standard <see cref="ApiResponse{T}"/>.
        /// </returns>
        [HttpPost("ask")]
        public async Task<ActionResult<ApiResponse<AskResponse>>> Ask(
            [FromBody] AskRequest request,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(
                    new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Validation failed.",
                        Data = null,
                        Errors = ModelState.ToDictionary(
                            x => x.Key,
                            x => x.Value?.Errors
                                    .Select(e => e.ErrorMessage)
                                    .ToArray()
                                ?? Array.Empty<string>())
                    });
            }

            var answer = await _aiService.AskAsync(
                request.Prompt,
                cancellationToken);

            return Ok(
                ApiResponse<AskResponse>.SuccessResponse(
                    new AskResponse { Answer = answer },
                    "AI response generated successfully."));
        }
    }
}
