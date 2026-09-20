using System.ComponentModel.DataAnnotations;

namespace CaseTrackerApplication.DTOs.AI
{
    /// <summary>
    /// Request body for POST /api/ai/ask.
    /// </summary>
    public class AskRequest
    {
        /// <summary>
        /// The question or instruction to send to the AI provider.
        /// </summary>
        [Required(ErrorMessage = "Prompt is required.")]
        [MinLength(1, ErrorMessage = "Prompt cannot be empty.")]
        [MaxLength(4000, ErrorMessage = "Prompt cannot exceed 4000 characters.")]
        public string Prompt { get; set; } = string.Empty;
    }
}
