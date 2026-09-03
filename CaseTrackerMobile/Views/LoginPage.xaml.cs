using System;
using Microsoft.Maui.Controls;
using CaseTrackerMobile.ViewModels;

namespace CaseTrackerMobile.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();

        // Resolve viewmodel from the app service provider
        BindingContext = App.Services.GetService(typeof(LoginViewModel)) as LoginViewModel;
    }

    private async void OnRegisterTapped(object sender, EventArgs e)
    {
        try
        {
            // Use absolute route to ensure Shell finds the Register ShellContent
            await Shell.Current.GoToAsync("//Register");
        }
        catch (Exception ex)
        {
            // Log or show navigation errors for easier diagnosis
            System.Diagnostics.Debug.WriteLine($"Navigation to Register failed: {ex}");
            await Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert("Navigation Error", "Unable to open registration page.", "OK");
        }
    }
}
