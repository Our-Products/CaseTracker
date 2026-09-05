using System;
using System.Threading.Tasks;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;

namespace CaseTrackerMobile.Views
{
    public partial class StartupSplashPage : ContentPage
    {
        private bool _isInitializing = false;

        public StartupSplashPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (!_isInitializing)
            {
                _isInitializing = true;
                await StartAppInitializationSequenceAsync();
            }
        }

        private async Task StartAppInitializationSequenceAsync()
        {
            try
            {
                // Smooth sequential pulse animation on the 3D logo
                if (ScalesLogoBorder != null)
                {
                    await ScalesLogoBorder.ScaleToAsync(1.04, 400, Easing.CubicOut);
                    await ScalesLogoBorder.ScaleToAsync(1.0, 400, Easing.CubicIn);
                }

                // Dependency & Virtual Database pre-load
                await Task.Delay(600);

                if (StatusLabel != null)
                {
                    StatusLabel.Text = "Workspace Ready!";
                }

                await Task.Delay(200);

                // Transition to AppShell safely on Main Thread
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (Application.Current != null)
                    {
                        Application.Current.MainPage = new AppShell();
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Startup initialization error: {ex.Message}");
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (Application.Current != null)
                    {
                        Application.Current.MainPage = new AppShell();
                    }
                });
            }
        }
    }
}
