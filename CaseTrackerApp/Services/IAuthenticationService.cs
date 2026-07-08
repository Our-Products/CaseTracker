using System.Threading.Tasks;

namespace CaseTrackerApp.Services;

public interface IAuthenticationService
{
    Task<bool> SignInAsync(string username, string password);
    Task SignOutAsync();
    Task<bool> IsSignedInAsync();
    Task<string?> GetAccessTokenAsync();
}
