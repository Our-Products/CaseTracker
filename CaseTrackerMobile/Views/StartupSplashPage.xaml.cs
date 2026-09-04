using System;
using System.Threading.Tasks;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;

namespace CaseTrackerMobile.Views
{
    public partial class StartupSplashPage : ContentPage
    {
        public StartupSplashPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await StartAppInitializationSequenceAsync();
        }

        private async Task StartAppInitializationSequenceAsync()
        {
            try
            {
                // Subtle pulse animation on the 3D logo
                if (ScalesLogoBorder != null)
                {
                    _ = ScalesLogoBorder.ScaleTo(1.05, 800, Easing.CubicInOut)
                        .ContinueWith(_ => MainThread.BeginInvokeOnMainThread(async () =>
                        {
                            await ScalesLogoBorder.ScaleTo(1.0, 800, Easing.CubicInOut);
                        }));
                }

                // Run background dependency boots & database check
                await Task.Run(async () =>
                {
                    // Simulated dependency / virtual database startup
                    await Task.Delay(1200);
                });

                if (StatusLabel != null)
                {
                    StatusLabel.Text = "Workspace Ready!";
                }

                await Task.Delay(300);

                // Seamless Window Swap to AppShell on Main UI Thread
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
