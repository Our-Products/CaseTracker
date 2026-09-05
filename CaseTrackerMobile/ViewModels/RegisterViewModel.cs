using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CaseTrackerApplication.DTOs;
using CaseTrackerMobile.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;

namespace CaseTrackerMobile.ViewModels
{
    public partial class RegisterViewModel : ObservableValidator
    {
        private readonly IAuthService _authService;

        public RegisterViewModel(IAuthService authService)
        {
            _authService = authService;

            States = new ObservableCollection<string>
            {
                "Tamil Nadu",
                "Puducherry"
            };

            BarCouncilName = "Bar Council of Tamil Nadu and Puducherry";
            EnrollmentDate = DateTime.Today;
        }

        #region Form Fields with Real-Time ObservableValidator

        private string _fullName = string.Empty;

        [Required(ErrorMessage = "Full Advocate Name is required.")]
        [MinLength(3, ErrorMessage = "Name must be at least 3 characters.")]
        public string FullName
        {
            get => _fullName;
            set
            {
                SetProperty(ref _fullName, value, true);
                OnPropertyChanged(nameof(FullNameError));
                OnPropertyChanged(nameof(HasFullNameError));
            }
        }

        public string FullNameError => GetErrors(nameof(FullName)).FirstOrDefault()?.ErrorMessage ?? string.Empty;
        public bool HasFullNameError => !string.IsNullOrEmpty(FullNameError);

        private string _mobileNumber = string.Empty;

        [Required(ErrorMessage = "Mobile number is required.")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Mobile number must be exactly 10 digits.")]
        public string MobileNumber
        {
            get => _mobileNumber;
            set
            {
                SetProperty(ref _mobileNumber, value, true);
                OnPropertyChanged(nameof(MobileNumberError));
                OnPropertyChanged(nameof(HasMobileNumberError));
            }
        }

        public string MobileNumberError => GetErrors(nameof(MobileNumber)).FirstOrDefault()?.ErrorMessage ?? string.Empty;
        public bool HasMobileNumberError => !string.IsNullOrEmpty(MobileNumberError);

        private string _email = string.Empty;

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        public string Email
        {
            get => _email;
            set
            {
                SetProperty(ref _email, value, true);
                OnPropertyChanged(nameof(EmailError));
                OnPropertyChanged(nameof(HasEmailError));
            }
        }

        public string EmailError => GetErrors(nameof(Email)).FirstOrDefault()?.ErrorMessage ?? string.Empty;
        public bool HasEmailError => !string.IsNullOrEmpty(EmailError);

        private string _barCouncilId = string.Empty;

        [Required(ErrorMessage = "Bar Council ID is required (e.g. TN/1234/2026).")]
        public string BarCouncilId
        {
            get => _barCouncilId;
            set
            {
                SetProperty(ref _barCouncilId, value, true);
                OnPropertyChanged(nameof(BarCouncilIdError));
                OnPropertyChanged(nameof(HasBarCouncilIdError));
            }
        }

        public string BarCouncilIdError => GetErrors(nameof(BarCouncilId)).FirstOrDefault()?.ErrorMessage ?? string.Empty;
        public bool HasBarCouncilIdError => !string.IsNullOrEmpty(BarCouncilIdError);

        private string _barCouncilName = string.Empty;
        public string BarCouncilName
        {
            get => _barCouncilName;
            set => SetProperty(ref _barCouncilName, value);
        }

        private DateTime _enrollmentDate = DateTime.Today;
        public DateTime EnrollmentDate
        {
            get => _enrollmentDate;
            set => SetProperty(ref _enrollmentDate, value);
        }

        private int _selectedRegisterTypeIndex = 0;
        public int SelectedRegisterTypeIndex
        {
            get => _selectedRegisterTypeIndex;
            set
            {
                SetProperty(ref _selectedRegisterTypeIndex, value);
                OnPropertyChanged(nameof(IsAssociates));
            }
        }

        public bool IsAssociates => SelectedRegisterTypeIndex == 1;

        private string _lawFirmName = string.Empty;
        public string LawFirmName
        {
            get => _lawFirmName;
            set => SetProperty(ref _lawFirmName, value);
        }

        private string _lawFirmRegistration = string.Empty;
        public string LawFirmRegistration
        {
            get => _lawFirmRegistration;
            set => SetProperty(ref _lawFirmRegistration, value);
        }

        private string _lawFirmAddress = string.Empty;
        public string LawFirmAddress
        {
            get => _lawFirmAddress;
            set => SetProperty(ref _lawFirmAddress, value);
        }

        private string _city = string.Empty;
        public string City
        {
            get => _city;
            set => SetProperty(ref _city, value);
        }

        private string _district = string.Empty;
        public string District
        {
            get => _district;
            set => SetProperty(ref _district, value);
        }

        private string _pincodeStr = string.Empty;
        public string PincodeStr
        {
            get => _pincodeStr;
            set => SetProperty(ref _pincodeStr, value);
        }

        public ObservableCollection<string> States { get; }

        private string _selectedState = "Tamil Nadu";
        public string SelectedState
        {
            get => _selectedState;
            set => SetProperty(ref _selectedState, value);
        }

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
                ValidateConfirmPassword();
            }
        }

        public string PasswordError => GetErrors(nameof(Password)).FirstOrDefault()?.ErrorMessage ?? string.Empty;
        public bool HasPasswordError => !string.IsNullOrEmpty(PasswordError);

        private string _confirmPassword = string.Empty;
        public string ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                SetProperty(ref _confirmPassword, value);
                ValidateConfirmPassword();
            }
        }

        private string _confirmPasswordError = string.Empty;
        public string ConfirmPasswordError
        {
            get => _confirmPasswordError;
            set
            {
                SetProperty(ref _confirmPasswordError, value);
                OnPropertyChanged(nameof(HasConfirmPasswordError));
            }
        }

        public bool HasConfirmPasswordError => !string.IsNullOrEmpty(ConfirmPasswordError);

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        #endregion

        private void ValidateConfirmPassword()
        {
            if (!string.IsNullOrEmpty(ConfirmPassword) && !ConfirmPassword.Equals(Password))
            {
                ConfirmPasswordError = "Passwords do not match.";
            }
            else
            {
                ConfirmPasswordError = string.Empty;
            }
        }

        #region Commands

        [RelayCommand]
        public async Task ExecuteRegisterAsync()
        {
            if (IsBusy) return;

            // Trigger real-time validation for all fields
            ValidateAllProperties();
            ValidateConfirmPassword();

            OnPropertyChanged(nameof(FullNameError));
            OnPropertyChanged(nameof(HasFullNameError));
            OnPropertyChanged(nameof(MobileNumberError));
            OnPropertyChanged(nameof(HasMobileNumberError));
            OnPropertyChanged(nameof(EmailError));
            OnPropertyChanged(nameof(HasEmailError));
            OnPropertyChanged(nameof(BarCouncilIdError));
            OnPropertyChanged(nameof(HasBarCouncilIdError));
            OnPropertyChanged(nameof(PasswordError));
            OnPropertyChanged(nameof(HasPasswordError));

            if (HasErrors || HasConfirmPasswordError)
            {
                await ShowAlertAsync("Validation Error", "Please complete all required advocate profile fields.");
                return;
            }

            IsBusy = true;

            try
            {
                int? parsedPincode = int.TryParse(PincodeStr, out var p) ? p : null;

                var req = new RegisterRequest
                {
                    FullName = FullName.Trim(),
                    MobileNumber = MobileNumber.Trim(),
                    Email = Email.Trim(),
                    Password = Password,
                    BarCouncilId = BarCouncilId.Trim(),
                    BarCouncilName = BarCouncilName.Trim(),
                    EnrollmentDate = EnrollmentDate,
                    RegisterType = (CaseTrackerApplication.DTOs.RegisterType)SelectedRegisterTypeIndex
                };

                if (IsAssociates)
                {
                    req.LawFirm = new LawFirmDto
                    {
                        FirmName = LawFirmName?.Trim() ?? string.Empty,
                        RegistrationNumber = string.IsNullOrWhiteSpace(LawFirmRegistration) ? null : LawFirmRegistration.Trim(),
                        AddressLine1 = string.IsNullOrWhiteSpace(LawFirmAddress) ? null : LawFirmAddress.Trim(),
                        City = string.IsNullOrWhiteSpace(City) ? null : City.Trim(),
                        District = string.IsNullOrWhiteSpace(District) ? null : District.Trim(),
                        State = SelectedState,
                        Pincode = parsedPincode
                    };
                }

                var result = await _authService.RegisterAsync(req);
                if (result == null)
                {
                    await ShowAlertAsync("Registration Failed", "Unable to register. Mobile number or Email may already exist.");
                    return;
                }

                if (!string.IsNullOrWhiteSpace(result.Token))
                {
                    await SecureStorage.Default.SetAsync("auth_token", result.Token);
                }

                await ShowAlertAsync("Success", "Advocate Profile registered successfully! Please login with your mobile number.");

                if (Shell.Current != null)
                    await Shell.Current.GoToAsync("//Login");
            }
            catch (Exception ex)
            {
                await ShowAlertAsync("Error", ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task CloseAsync()
        {
            if (Shell.Current != null)
                await Shell.Current.GoToAsync("//Login");
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
