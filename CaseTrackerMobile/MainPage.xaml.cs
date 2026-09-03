using System;
using Microsoft.Maui.Controls;
using CaseTrackerMobile.Views;

namespace CaseTrackerMobile
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        //private async void LogoutButton_Clicked(object sender, EventArgs e)
        //{
        //    //Preferences.Remove("AuthToken");
        //    //Preferences.Remove("UserId");
        //    //Preferences.Remove("UserName");

        //    await Shell.Current.GoToAsync("//LoginPage");
        //}

    }
}
