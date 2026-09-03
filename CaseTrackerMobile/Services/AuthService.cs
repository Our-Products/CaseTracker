using CaseTrackerApplication.DTOs;
using System.Net.Http.Json;
using System.Text.Json;

namespace CaseTrackerMobile.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _http;

        public AuthService(HttpClient http)
        {
            _http = http;
        }

        public async Task<string?> LoginAsync(LoginRequest request)
        {
            // POST to api/auth/login (change path to match your backend)
            var response = await _http.PostAsJsonAsync("api/auth/login", request);
            if (!response.IsSuccessStatusCode)
                return null;

            // Try to parse a token property from the JSON response
            var content = await response.Content.ReadAsStringAsync();
            try
            {
                using var doc = JsonDocument.Parse(content);
                if (doc.RootElement.TryGetProperty("token", out var tokenProp))
                {
                    return tokenProp.GetString();
                }

                // Fallback: if server returns the token as plain string
                return content?.Trim('"');
            }
            catch
            {
                return null;
            }
        }

        public async Task<AuthResult?> RegisterAsync(RegisterRequest request)
        {
            var response = await _http.PostAsJsonAsync("api/auth/register", request);
            if (!response.IsSuccessStatusCode)
                return null;

            try
            {
                // Use built-in JSON deserialization to parse the AuthResult
                var result = await response.Content.ReadFromJsonAsync<CaseTrackerApplication.DTOs.AuthResult>();
                return result;
            }
            catch
            {
                return null;
            }
        }
    }
}
