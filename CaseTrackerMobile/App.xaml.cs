using System;
using Microsoft.Maui.Controls;

namespace CaseTrackerMobile
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Set AppShell directly as root MainPage to prevent Android activity lifecycle crashes
            MainPage = new AppShell();
        }

        // Service provider populated in MauiProgram so pages/viewmodels can resolve services
        public static IServiceProvider Services { get; internal set; } = null!;
    }
}