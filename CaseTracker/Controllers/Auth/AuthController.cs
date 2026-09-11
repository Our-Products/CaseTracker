using CaseTrackerApplication.DTOs.Auth;
using CaseTrackerApplication.DTOs.Common;
using CaseTrackerApplication.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CaseTracker.Extensions;

namespace CaseTracker.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Register a new lawyer or organization.
        /// </summary>
        /// <param name="request">
        /// Registration details.
        /// </param>
        /// <returns>
        /// Authentication token and user details.
        /// </returns>
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<AuthResult>>> Register(
            [FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(
                    new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Validation failed.",
                        Data = null,
                        Errors = ModelState
                            .ToDictionary(
                                x => x.Key,
                                x => x.Value?.Errors
                                    .Select(e => e.ErrorMessage)
                                    .ToArray() ?? Array.Empty<string>()
                            )
                    });
            }

            var result =
                await _authService.RegisterAsync(request);

            return Ok(
                new ApiResponse<AuthResult>
                {
                    Success = true,
                    Message = "Registration successful.",
                    Data = result,
                    Errors = null
                });
        }

        /// <summary>
        /// Login with mobile number and password.
        /// </summary>
        /// <param name="request">
        /// Login credentials.
        /// </param>
        /// <returns>
        /// Authentication token and user details.
        /// </returns>
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<AuthResult>>> Login(
            [FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(
                    new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Validation failed.",
                        Data = null,
                        Errors = ModelState
                            .ToDictionary(
                                x => x.Key,
                                x => x.Value?.Errors
                                    .Select(e => e.ErrorMessage)
                                    .ToArray() ?? Array.Empty<string>()
                            )
                    });
            }

            var result =
                await _authService.LoginAsync(request);

            return Ok(
                new ApiResponse<AuthResult>
                {
                    Success = true,
                    Message = "Login successful.",
                    Data = result,
                    Errors = null
                });
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult GetCurrentUser()
        {
            var userId = User.GetUserId();
            if (!userId.HasValue)
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "User identity claim not found.",
                    Data = null
                });
            }

            var roles = User.GetRoles();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "User profile retrieved successfully.",
                Data = new
                {
                    UserId = userId.Value,
                    Roles = roles
                }
            });
        }
    }
}

