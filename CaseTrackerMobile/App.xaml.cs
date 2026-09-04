using System;
using System.Threading.Tasks;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace CaseTrackerMobile
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Establish lightweight, completely blank ContentPage with #0D1117 dark baseline
            MainPage = new ContentPage
            {
                BackgroundColor = Color.FromArgb("#0D1117")
            };
        }

        // Service provider populated in MauiProgram so pages/viewmodels can resolve services
        public static IServiceProvider Services { get; internal set; } = null!;

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = base.CreateWindow(activationState);
            window.Page ??= MainPage;
            return window;
        }

        protected override async void OnStart()
        {
            base.OnStart();
            await RunBackgroundAppBootSequenceAsync();
        }

        private async Task RunBackgroundAppBootSequenceAsync()
        {
            try
            {
                // Asynchronous background task for app boots & dependency setups
                await Task.Run(async () =>
                {
                    await Task.Delay(100);
                });

                // Swap MainPage on Main UI Thread to StartupSplashPage
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    MainPage = new Views.StartupSplashPage();
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"App boot sequence error: {ex.Message}");
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    MainPage = new Views.StartupSplashPage();
                });
            }
        }
    }
}