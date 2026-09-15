using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CaseTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VersionController : ControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetVersion()
        {
            return Ok(new
            {
                service = "CaseTracker API",
                version = "1.0.1",
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
                deployedAt = DateTime.UtcNow.ToString("o"),
                branch = "master (tested from dev/soorya)"
            });
        }
    }
}
