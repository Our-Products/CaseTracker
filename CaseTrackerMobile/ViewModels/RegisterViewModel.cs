using CaseTrackerApplication.DTOs;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CaseTrackerMobile.ViewModels
{
    public class RegisterViewModel : BindableObject
    {
        private readonly CaseTrackerMobile.Services.IAuthService _authService;

        public RegisterViewModel(CaseTrackerMobile.Services.IAuthService authService)
        {
            _authService = authService;

            States = new ObservableCollection<string>
            {
                "Tamil Nadu",
                "Puducherry"
            };

            RegisterCommand = new Command(async () => await ExecuteRegister());
            CloseCommand = new Command(async () => await ExecuteClose());
        }

        string _fullName = string.Empty;
        public string FullName { get => _fullName; set { _fullName = value; OnPropertyChanged(); } }

        string _mobileNumber = string.Empty;
        public string MobileNumber { get => _mobileNumber; set { _mobileNumber = value; OnPropertyChanged(); } }

        string _email = string.Empty;
        public string Email { get => _email; set { _email = value; OnPropertyChanged(); } }

        string _password = string.Empty;
        public string Password { get => _password; set { _password = value; OnPropertyChanged(); } }

        string _confirmPassword = string.Empty;
        public string ConfirmPassword { get => _confirmPassword; set { _confirmPassword = value; OnPropertyChanged(); } }

        string _barCouncilNumber = string.Empty;
        public string BarCouncilNumber { get => _barCouncilNumber; set { _barCouncilNumber = value; OnPropertyChanged(); } }

        // Registration type: 0 = Individual, 1 = Associates/Organization
        int _selectedRegisterTypeIndex = 0;
        public int SelectedRegisterTypeIndex { get => _selectedRegisterTypeIndex; set { _selectedRegisterTypeIndex = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsAssociates)); } }

        public bool IsAssociates => SelectedRegisterTypeIndex == 1;

        string _lawFirmName = string.Empty;
        public string LawFirmName { get => _lawFirmName; set { _lawFirmName = value; OnPropertyChanged(); } }

        string _lawFirmRegistration = string.Empty;
        public string LawFirmRegistration { get => _lawFirmRegistration; set { _lawFirmRegistration = value; OnPropertyChanged(); } }

        string _lawFirmAddress = string.Empty;
        public string LawFirmAddress { get => _lawFirmAddress; set { _lawFirmAddress = value; OnPropertyChanged(); } }

        public ObservableCollection<string> States { get; }

        string _selectedState;
        public string SelectedState { get => _selectedState; set { _selectedState = value; OnPropertyChanged(); } }

        public ICommand RegisterCommand { get; }
        public ICommand CloseCommand { get; }

        bool _isBusy;
        public bool IsBusy { get => _isBusy; set { _isBusy = value; OnPropertyChanged(); } }

        public async System.Threading.Tasks.Task ExecuteRegister()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                if (string.IsNullOrWhiteSpace(FullName) || string.IsNullOrWhiteSpace(MobileNumber) || string.IsNullOrWhiteSpace(Email))
                {
                    await Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert("Validation", "Please fill required fields (name, mobile, email).", "OK");
                    return;
                }

                // Password validation
                if (string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(ConfirmPassword))
                {
                    await Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert("Validation", "Please enter password and confirm it.", "OK");
                    return;
                }

                if (Password.Length < 6)
                {
                    await Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert("Validation", "Password must be at least 6 characters long.", "OK");
                    return;
                }

                if (!Password.Equals(ConfirmPassword))
                {
                    await Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert("Validation", "Password and Confirm Password do not match.", "OK");
                    return;
                }

                // Call backend registration API via IAuthService
                var req = new RegisterRequest
                {
                    FullName = FullName.Trim(),
                    MobileNumber = MobileNumber.Trim(),
                    Email = string.IsNullOrWhiteSpace(Email) ? null : Email.Trim(),
                    Password = Password,
                    RegisterType = (CaseTrackerApplication.DTOs.RegisterType)SelectedRegisterTypeIndex
                };

                if (IsAssociates)
                {
                    req.LawFirm = new CaseTrackerApplication.DTOs.LawFirmDto
                    {
                        FirmName = LawFirmName?.Trim() ?? string.Empty,
                        RegistrationNumber = string.IsNullOrWhiteSpace(LawFirmRegistration) ? null : LawFirmRegistration.Trim(),
                        AddressJson = string.IsNullOrWhiteSpace(LawFirmAddress) ? null : LawFirmAddress.Trim()
                    };
                }

                var result = await _authService.RegisterAsync(req);
                if (result == null)
                {
                    await Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert("Registration Failed", "Unable to register. Please try again.", "OK");
                    return;
                }

                // Optionally store token if provided
                if (!string.IsNullOrWhiteSpace(result.Token))
                {
                    await SecureStorage.Default.SetAsync("auth_token", result.Token);
                }

                await Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert("Success", "Registration successful. Please login.", "OK");

                // Navigate back to Login
                await Shell.Current.GoToAsync("//Login");
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

        async System.Threading.Tasks.Task ExecuteClose()
        {
            try
            {
                await Shell.Current.GoToAsync("//Login");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Close navigation failed: {ex}");
                await Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert("Navigation Error", "Unable to return to login.", "OK");
            }
        }
    }
}
