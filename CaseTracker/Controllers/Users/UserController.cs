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