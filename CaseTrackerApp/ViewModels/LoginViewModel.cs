using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Input;
using CaseTrackerApp.Services;

namespace CaseTrackerApp.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    private readonly IAuthenticationService _auth;
    private readonly INavigationService _nav;

    [ObservableProperty]
    private string username = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    public IAsyncRelayCommand SignInCommand { get; }

    public LoginViewModel(IAuthenticationService auth, INavigationService nav)
    {
        _auth = auth;
        _nav = nav;
        SignInCommand = new AsyncRelayCommand(async () => await SignInAsync());
    }

    private async Task SignInAsync()
    {
        var ok = await _auth.SignInAsync(Username, Password);
        if (ok)
        {
            await _nav.NavigateToAsync("//Dashboard");
        }
        else
        {
            // Avoid using deprecated Application.MainPage; use current window if available
            var page = Application.Current?.Windows?.FirstOrDefault()?.Page;
            if (page != null)
                await page.DisplayAlertAsync("Login", "Invalid credentials", "OK");
        }
    }
}
