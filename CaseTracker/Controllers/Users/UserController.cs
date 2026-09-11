using CaseTracker.Constants;
using CaseTracker.Extensions;
using CaseTrackerApplication.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CaseTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/user
        [Authorize(Roles = AppRoles.SuperAdminOrAdmin)]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllAsync();

            return Ok(users);
        }

        // GET: api/user/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            if (!User.CanAccessUser(id))
            {
                return Forbid();
            }

            var user = await _userService.GetByIdAsync(id);

            if (user == null)
            {
                return NotFound(new
                {
                    message = "User not found."
                });
            }

            return Ok(user);
        }

        // GET: api/user/mobile/{mobileNumber}
        [HttpGet("mobile/{mobileNumber}")]
        public async Task<IActionResult> GetByMobileNumber(
            string mobileNumber)
        {
            if (!User.IsAdmin())
            {
                var currentUserId = User.GetUserId();
                if (!currentUserId.HasValue)
                {
                    return Forbid();
                }

                var currentUser = await _userService.GetByIdAsync(currentUserId.Value);
                if (currentUser == null || !string.Equals(currentUser.MobileNumber, mobileNumber, StringComparison.OrdinalIgnoreCase))
                {
                    return Forbid();
                }
            }

            var user =
                await _userService
                    .GetByMobileNumberAsync(mobileNumber);

            if (user == null)
            {
                return NotFound(new
                {
                    message = "User not found."
                });
            }

            return Ok(user);
        }

        // GET: api/user/mobile-exists/{mobileNumber}
        [AllowAnonymous]
        [HttpGet("mobile-exists/{mobileNumber}")]
        public async Task<IActionResult> MobileNumberExists(
            string mobileNumber)
        {
            var exists =
                await _userService
                    .MobileNumberExistsAsync(mobileNumber);

            return Ok(new
            {
                exists
            });
        }

        // GET: api/user/email-exists?email=test@example.com
        [AllowAnonymous]
        [HttpGet("email-exists")]
        public async Task<IActionResult> EmailExists(
            [FromQuery] string email)
        {
            var exists =
                await _userService
                    .EmailExistsAsync(email);

            return Ok(new
            {
                exists
            });
        }
    }
}