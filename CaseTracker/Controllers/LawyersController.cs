using Application.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CaseTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]  // ← ALL endpoints require JWT token authentication
    public class LawyersController : ControllerBase
    {
        private readonly ILawyerRepository _lawyerRepository;

        public LawyersController(ILawyerRepository lawyerRepository)
        {
            _lawyerRepository = lawyerRepository;
        }

        /// <summary>
        /// Get all lawyers (requires authentication)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Lawyer>>> GetAll()
        {
            var list = await _lawyerRepository.GetAllAsync();
            return Ok(list);
        }

        /// <summary>
        /// Get lawyer by ID (requires authentication)
        /// </summary>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<Lawyer>> Get(Guid id)
        {
            var lawyer = await _lawyerRepository.GetByIdAsync(id);
            if (lawyer == null)
                return NotFound();

            return Ok(lawyer);
        }

        /// <summary>
        /// Create a new lawyer (requires authentication)
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Lawyer>> Create(Lawyer input)
        {
            input.LawyerId = Guid.NewGuid();
            input.CreatedAt = DateTime.UtcNow;
            input.UpdatedAt = DateTime.UtcNow;

            await _lawyerRepository.AddAsync(input);
            await _lawyerRepository.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = input.LawyerId }, input);
        }

        /// <summary>
        /// Update a lawyer (requires authentication)
        /// </summary>
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, Lawyer input)
        {
            if (id != input.LawyerId)
                return BadRequest();

            var existingLawyer = await _lawyerRepository.GetByIdAsync(id);
            if (existingLawyer == null)
                return NotFound();

            input.UpdatedAt = DateTime.UtcNow;

            await _lawyerRepository.UpdateAsync(input);
            await _lawyerRepository.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Delete a lawyer (requires authentication)
        /// </summary>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var lawyer = await _lawyerRepository.GetByIdAsync(id);
            if (lawyer == null)
                return NotFound();

            await _lawyerRepository.DeleteAsync(id);
            await _lawyerRepository.SaveChangesAsync();

            return NoContent();
        }
    }
}
