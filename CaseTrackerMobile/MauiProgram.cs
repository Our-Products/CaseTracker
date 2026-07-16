using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using CaseTrackerMobile.Services;
using CaseTrackerMobile.ViewModels;
using System;

namespace CaseTrackerMobile
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            // Register services
            // Replace the BaseAddress with your API endpoint
            builder.Services.AddSingleton(new System.Net.Http.HttpClient { BaseAddress = new Uri("https://your-api-base/") });
            builder.Services.AddSingleton<IAuthService, AuthService>();
            builder.Services.AddTransient<LoginViewModel>();

            var app = builder.Build();

            // expose the service provider for pages/viewmodels
            App.Services = app.Services;

            return app;
        }
    }
}
