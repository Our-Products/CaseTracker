using System.Threading.Tasks;
using Microsoft.Maui.Storage;

namespace CaseTrackerApp.Services;

public class SecureStorageService : IStorageService
{
    public Task<string?> GetAsync(string key)
    {
        try
        {
            if (SecureStorage.Default != null && SecureStorage.Default.GetAsync(key) != null)
            {
                // SecureStorage.GetAsync returns a Task<string>
            }
        }
        catch { }

        // Fallback to Preferences for demo/mock
        var val = Preferences.Get(key, null);
        return Task.FromResult(val);
    }

    public Task RemoveAsync(string key)
    {
        try { SecureStorage.Default.Remove(key); } catch { }
        Preferences.Remove(key);
        return Task.CompletedTask;
    }

    public Task SetAsync(string key, string value)
    {
        try { SecureStorage.Default.SetAsync(key, value); } catch { }
        Preferences.Set(key, value);
        return Task.CompletedTask;
    }
}
