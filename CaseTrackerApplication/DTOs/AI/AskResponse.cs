namespace CaseTrackerApplication.DTOs.AI
{
    /// <summary>
    /// Response body returned by POST /api/ai/ask.
    /// </summary>
    public class AskResponse
    {
        /// <summary>
        /// The AI-generated answer to the submitted prompt.
        /// </summary>
        public string Answer { get; set; } = string.Empty;
    }
}
