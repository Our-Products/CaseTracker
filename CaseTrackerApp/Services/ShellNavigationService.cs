using Microsoft.Maui.Controls;

namespace CaseTrackerApp.Services;

public class ShellNavigationService : INavigationService
{
    public Task GoBackAsync()
    {
        return Shell.Current.GoToAsync("..", true);
    }

    public Task NavigateToAsync(string route, IDictionary<string, object>? parameters = null)
    {
        if (parameters == null || parameters.Count == 0)
            return Shell.Current.GoToAsync(route, true);

        // build query string for simple parameters
        var qs = string.Join("&", parameters.Select(kvp => $"{kvp.Key}={System.Net.WebUtility.UrlEncode(kvp.Value?.ToString() ?? string.Empty)}"));
        var full = route.Contains("?") ? route + "&" + qs : route + "?" + qs;
        return Shell.Current.GoToAsync(full, true);
    }
}
