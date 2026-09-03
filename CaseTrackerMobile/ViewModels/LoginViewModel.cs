using CaseTrackerApplication.DTOs;
using CaseTrackerMobile.Services;
using System.Windows.Input;

namespace CaseTrackerMobile.ViewModels
{
    public class LoginViewModel : BindableObject
    {
        private readonly IAuthService _authService;

        public LoginViewModel(IAuthService authService)
        {
            _authService = authService;
            LoginCommand = new Command(async () => await ExecuteLogin());
        }

        string _mobile = string.Empty;
        public string Mobile
        {
            get => _mobile;
            set { _mobile = value; OnPropertyChanged(); }
        }

        string _password = string.Empty;
        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set { _isBusy = value; OnPropertyChanged(); }
        }

        public ICommand LoginCommand { get; }

        public async Task ExecuteLogin()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                if (string.IsNullOrWhiteSpace(Mobile) || string.IsNullOrWhiteSpace(Password))
                {
                    await Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert("Validation", "Please enter mobile number and password.", "OK");
                    return;
                }

                var request = new LoginRequest { MobileNumber = Mobile.Trim(), Password = Password };
                var token = await _authService.LoginAsync(request);
                if (token == null)
                {
                    await Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert("Login Failed", "Invalid credentials or server error.", "OK");
                    return;
                }

                // Store token securely
                await SecureStorage.Default.SetAsync("auth_token", token);

                // Navigate to main page (absolute route)
                await Shell.Current.GoToAsync("//MainPage");
            }
            catch (Exception ex)
            {
                await Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
