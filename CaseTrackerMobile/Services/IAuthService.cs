using System.Threading.Tasks;
using Application.DTOs;

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
        Task<Application.DTOs.AuthResult?> RegisterAsync(Application.DTOs.RegisterRequest request);
    }
}
