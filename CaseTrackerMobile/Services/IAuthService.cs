using CaseTrackerApplication.DTOs.Auth;

namespace CaseTrackerMobile.Services
{
    public interface IAuthService
    {
        /// <summary>
        /// Attempts to log in and returns a token string on success or null on failure.
        /// </summary>
        Task<string?> LoginAsync(LoginRequest request);

        /// <summary>
        /// Attempts to register a new user and returns an AuthResult on success or null on failure.
        /// </summary>
        Task<AuthResult?> RegisterAsync(RegisterRequest request);
    }
}
