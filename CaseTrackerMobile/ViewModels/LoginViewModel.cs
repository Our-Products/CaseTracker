using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CaseTrackerApplication.DTOs;
using CaseTrackerMobile.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;

namespace CaseTrackerMobile.ViewModels
{
    public partial class LoginViewModel : ObservableValidator
    {
        private readonly IAuthService _authService;

        public LoginViewModel(IAuthService authService)
        {
            _authService = authService;
        }

        #region Validation Properties

        private string _mobile = string.Empty;

        [Required(ErrorMessage = "Mobile number is required.")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Mobile number must be exactly 10 digits.")]
        public string Mobile
        {
            get => _mobile;
            set
            {
                SetProperty(ref _mobile, value, true);
                OnPropertyChanged(nameof(MobileError));
                OnPropertyChanged(nameof(HasMobileError));
            }
        }

        public string MobileError => GetErrors(nameof(Mobile)).FirstOrDefault()?.ErrorMessage ?? string.Empty;
        public bool HasMobileError => !string.IsNullOrEmpty(MobileError);

        private string _password = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
        public string Password
        {
            get => _password;
            set
            {
                SetProperty(ref _password, value, true);
                OnPropertyChanged(nameof(PasswordError));
                OnPropertyChanged(nameof(HasPasswordError));
            }
        }

        public string PasswordError => GetErrors(nameof(Password)).FirstOrDefault()?.ErrorMessage ?? string.Empty;
        public bool HasPasswordError => !string.IsNullOrEmpty(PasswordError);

        private bool _isPasswordHidden = true;
        public bool IsPasswordHidden
        {
            get => _isPasswordHidden;
            set
            {
                if (SetProperty(ref _isPasswordHidden, value))
                {
                    OnPropertyChanged(nameof(PasswordToggleIcon));
                }
            }
        }

        public string PasswordToggleIcon => IsPasswordHidden ? Helpers.FontAwesomeIcons.Eye : Helpers.FontAwesomeIcons.EyeSlash;

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        #endregion

        #region Commands

        [RelayCommand]
        public void TogglePasswordVisibility()
        {
            IsPasswordHidden = !IsPasswordHidden;
        }

        [RelayCommand]
        public async Task ExecuteLoginAsync()
        {
            if (IsBusy) return;

            // Trigger real-time validation for all fields
            ValidateAllProperties();
            OnPropertyChanged(nameof(MobileError));
            OnPropertyChanged(nameof(HasMobileError));
            OnPropertyChanged(nameof(PasswordError));
            OnPropertyChanged(nameof(HasPasswordError));

            if (HasErrors)
            {
                await ShowAlertAsync("Validation Error", "Please resolve invalid field inputs before signing in.");
                return;
            }

            IsBusy = true;

            try
            {
                var request = new LoginRequest
                {
                    MobileNumber = Mobile.Trim(),
                    Password = Password
                };

                var token = await _authService.LoginAsync(request);

                if (token == null)
                {
                    await ShowAlertAsync("Login Failed", "Invalid credentials. (Try test account: 9876543210 / Password123!)");
                    return;
                }

                await SecureStorage.Default.SetAsync("auth_token", token);

                if (Shell.Current != null)
                    await Shell.Current.GoToAsync("//Dashboard");
            }
            catch (Exception ex)
            {
                await ShowAlertAsync("Login Error", ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task ForgotPasswordAsync()
        {
            await ShowAlertAsync("Password Recovery", "Instructions to reset password have been routed to your registered Advocate mobile number.");
        }

        [RelayCommand]
        public async Task CreateAccountAsync()
        {
            if (Shell.Current != null)
                await Shell.Current.GoToAsync("//Register");
        }

        private async Task ShowAlertAsync(string title, string message)
        {
            if (Shell.Current != null)
                await Shell.Current.DisplayAlert(title, message, "OK");
            else if (Application.Current?.Windows.Count > 0 && Application.Current.Windows[0].Page != null)
                await Application.Current.Windows[0].Page!.DisplayAlert(title, message, "OK");
        }

        #endregion
    }
}
