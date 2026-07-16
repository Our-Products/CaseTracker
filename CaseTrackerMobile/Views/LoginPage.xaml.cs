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
}
