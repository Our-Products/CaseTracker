using CaseTrackerApplication.DTOs;
using CaseTrackerApplication.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        /// Register a new lawyer
        /// </summary>
        /// <param name="request">Registration details</param>
        /// <returns>Authentication token and expiry</returns>
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<AuthResult>> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RegisterAsync(request);
            return Ok(result);
            // Exceptions automatically handled by GlobalExceptionHandlerMiddleware
        }

        /// <summary>
        /// Login with mobile number and password
        /// </summary>
        /// <param name="request">Login credentials</param>
        /// <returns>Authentication token and expiry</returns>
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<AuthResult>> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.LoginAsync(request);
            return Ok(result);
            // Exceptions automatically handled by GlobalExceptionHandlerMiddleware
        }
    }
}
