using Microsoft.Maui.Controls;
using CaseTrackerMobile.ViewModels;

namespace CaseTrackerMobile.Views;

public partial class RegisterPage : ContentPage
{
    public RegisterPage()
    {
        InitializeComponent();

        // Resolve viewmodel from the app service provider
        BindingContext = App.Services.GetService(typeof(RegisterViewModel)) as RegisterViewModel;
    }

    private async void OnCloseClicked(object sender, EventArgs e)
    {
        try
        {
            await Shell.Current.GoToAsync("//Login");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"OnCloseClicked navigation failed: {ex}");
            await Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert("Navigation Error", "Unable to return to login.", "OK");
        }
    }
}
