using CaseTrackerApplication.DTOs;

namespace CaseTrackerApplication.Interfaces.Services
{
    public interface IAuthService
    {
        Task<AuthResult> RegisterAsync(RegisterRequest request);

        Task<AuthResult> LoginAsync(LoginRequest request);
    }
}
