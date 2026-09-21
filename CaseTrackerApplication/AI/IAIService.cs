namespace CaseTrackerApplication.AI
{
    /// <summary>
    /// Abstraction for AI question-answer capabilities.
    /// The Application layer depends only on this interface;
    /// the concrete provider (Ollama, Azure OpenAI, Gemini …)
    /// lives entirely in the Infrastructure layer.
    /// </summary>
    public interface IAIService
    {
        /// <summary>
        /// Sends a prompt to the configured AI provider and returns the generated answer.
        /// </summary>
        /// <param name="prompt">The user's question or instruction.</param>
        /// <param name="cancellationToken">Propagates cancellation from the HTTP request.</param>
        /// <returns>The AI-generated answer string.</returns>
        Task<string> AskAsync(
            string prompt,
            CancellationToken cancellationToken = default);
    }
}
