using System.Threading.Tasks;
using CaseTrackerApp.Services;
using Microsoft.Maui.Storage;

namespace CaseTrackerApp.Services;

public class MockAuthenticationService : IAuthenticationService
{
    private const string TokenKey = "ct_auth_token";

    public Task<string?> GetAccessTokenAsync()
    {
        var token = Preferences.Get(TokenKey, null);
        return Task.FromResult(token);
    }

    public Task<bool> IsSignedInAsync()
    {
        var exists = !string.IsNullOrEmpty(Preferences.Get(TokenKey, string.Empty));
        return Task.FromResult(exists);
    }

    public Task SignOutAsync()
    {
        Preferences.Remove(TokenKey);
        return Task.CompletedTask;
    }

    public Task<bool> SignInAsync(string username, string password)
    {
        // Mock credentials: admin / Password123
        if (username == "admin" && password == "Password123")
        {
            Preferences.Set(TokenKey, "mock-jwt-token");
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }
}
