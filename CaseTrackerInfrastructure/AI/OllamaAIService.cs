using CaseTrackerApplication.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CaseTrackerInfrastructure.AI
{
    /// <summary>
    /// Ollama-backed implementation of <see cref="IAIService"/>.
    ///
    /// Configuration keys read from IConfiguration:
    ///   AI:BaseUrl  — Ollama server base URL  (e.g. http://localhost:11434)
    ///   AI:Model    — Model tag to use         (e.g. llama3.2)
    ///
    /// To swap to a different provider in a later phase, implement IAIService
    /// in a new class and update the DI registration — no changes needed
    /// in the controller or application layer.
    /// </summary>
    public class OllamaAIService : IAIService
    {
        // Named HTTP client registered in InfrastructureServiceExtensions.
        private const string HttpClientName = "Ollama";

        // Ollama REST path for non-streaming text generation.
        private const string GeneratePath = "/api/generate";

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<OllamaAIService> _logger;

        public OllamaAIService(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<OllamaAIService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<string> AskAsync(
            string prompt,
            CancellationToken cancellationToken = default)
        {
            var model = _configuration["AI:Model"]
                ?? throw new InvalidOperationException(
                    "AI model is not configured. Add 'AI:Model' to appsettings.");

            // Build the Ollama generate request.
            // stream:false → Ollama returns a single JSON object instead of NDJSON chunks.
            var requestBody = new OllamaGenerateRequest
            {
                Model = model,
                Prompt = prompt,
                Stream = false
            };

            var httpClient = _httpClientFactory.CreateClient(HttpClientName);

            _logger.LogInformation(
                "Sending request to Ollama. Model: {Model}",
                model);

            HttpResponseMessage httpResponse;

            try
            {
                httpResponse = await httpClient.PostAsJsonAsync(
                    GeneratePath,
                    requestBody,
                    cancellationToken);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(
                    ex,
                    "HTTP request to Ollama failed. " +
                    "Ensure Ollama is running and AI:BaseUrl is correct.");

                throw new InvalidOperationException(
                    "The AI service is currently unavailable. " +
                    "Please try again later.", ex);
            }
            catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogWarning(
                    ex,
                    "Request to Ollama timed out.");

                throw new InvalidOperationException(
                    "The AI service did not respond in time. " +
                    "Please try again later.", ex);
            }

            if (!httpResponse.IsSuccessStatusCode)
            {
                _logger.LogError(
                    "Ollama returned a non-success status code: {StatusCode}",
                    (int)httpResponse.StatusCode);

                throw new InvalidOperationException(
                    "The AI service returned an unexpected error. " +
                    "Please try again later.");
            }

            OllamaGenerateResponse? ollamaResponse;

            try
            {
                ollamaResponse = await httpResponse.Content
                    .ReadFromJsonAsync<OllamaGenerateResponse>(
                        cancellationToken: cancellationToken);
            }
            catch (JsonException ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to deserialize Ollama response.");

                throw new InvalidOperationException(
                    "The AI service returned an unreadable response. " +
                    "Please try again later.", ex);
            }

            if (ollamaResponse is null || string.IsNullOrWhiteSpace(ollamaResponse.Response))
            {
                _logger.LogWarning("Ollama returned an empty response.");

                throw new InvalidOperationException(
                    "The AI service returned an empty answer. " +
                    "Please try again.");
            }

            _logger.LogInformation("Ollama response received successfully.");

            return ollamaResponse.Response.Trim();
        }

        // ---------------------------------------------------------------
        // Private Ollama DTO types — kept internal to the infrastructure.
        // ---------------------------------------------------------------

        private sealed class OllamaGenerateRequest
        {
            [JsonPropertyName("model")]
            public string Model { get; set; } = string.Empty;

            [JsonPropertyName("prompt")]
            public string Prompt { get; set; } = string.Empty;

            /// <summary>
            /// Must be false so Ollama returns a single JSON response
            /// rather than a stream of NDJSON chunks.
            /// </summary>
            [JsonPropertyName("stream")]
            public bool Stream { get; set; } = false;
        }

        private sealed class OllamaGenerateResponse
        {
            [JsonPropertyName("model")]
            public string Model { get; set; } = string.Empty;

            [JsonPropertyName("response")]
            public string Response { get; set; } = string.Empty;

            [JsonPropertyName("done")]
            public bool Done { get; set; }
        }
    }
}
