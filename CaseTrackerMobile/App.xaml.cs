using System;
using Microsoft.Maui.Controls;

namespace CaseTrackerMobile
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Set StartupSplashPage directly as initial MainPage with #0D1117 baseline
            MainPage = new Views.StartupSplashPage();
        }

        // Service provider populated in MauiProgram so pages/viewmodels can resolve services
        public static IServiceProvider Services { get; internal set; } = null!;

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = base.CreateWindow(activationState);
            window.Page ??= MainPage;
            return window;
        }
    }
}