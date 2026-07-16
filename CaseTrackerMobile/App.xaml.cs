using Microsoft.Extensions.DependencyInjection;

namespace CaseTrackerMobile
{
    public partial class App : Microsoft.Maui.Controls.Application
    {
        public App()
        {
            InitializeComponent();
        }

        // Service provider populated in MauiProgram so pages/viewmodels can resolve services
        public static IServiceProvider Services { get; internal set; } = null!;

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}