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
                    // Add Font Awesome Solid font (fa-solid-900.ttf in Resources/Fonts)
                    fonts.AddFont("fa-solid-900.ttf", "FontAwesomeSolid");
                });

            // Configure global handler mappings to remove native underlines & borders
            Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("NoUnderline", (h, v) =>
            {
#if ANDROID
                h.PlatformView.Background = null;
                h.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
#elif IOS || MACCATALYST
                h.PlatformView.BorderStyle = UIKit.UITextBorderStyle.None;
#elif WINDOWS
                h.PlatformView.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
#endif
            });

            Microsoft.Maui.Handlers.PickerHandler.Mapper.AppendToMapping("NoUnderline", (h, v) =>
            {
#if ANDROID
                h.PlatformView.Background = null;
                h.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
#elif IOS || MACCATALYST
                h.PlatformView.BorderStyle = UIKit.UITextBorderStyle.None;
#elif WINDOWS
                h.PlatformView.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
#endif
            });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            // Register services
            // Configure HttpClient BaseAddress per platform for local development and allow
            // bypassing TLS validation in DEBUG when using emulators. Adjust ports/host as needed.
#if DEBUG
            string apiBaseAddress;
            var platform = Microsoft.Maui.Devices.DeviceInfo.Platform;
            if (platform == Microsoft.Maui.Devices.DevicePlatform.Android)
            {
                // Android emulator (AVD) routes host localhost to 10.0.2.2
                apiBaseAddress = "https://10.0.2.2:7232/";
            }
            else if (platform == Microsoft.Maui.Devices.DevicePlatform.WinUI || platform == Microsoft.Maui.Devices.DevicePlatform.MacCatalyst || platform == Microsoft.Maui.Devices.DevicePlatform.iOS)
            {
                apiBaseAddress = "https://localhost:7232/";
            }
            else
            {
                apiBaseAddress = "https://localhost:7232/";
            }

            var handler = new System.Net.Http.HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true; // DEBUG only: accept dev certs
            builder.Services.AddSingleton(new System.Net.Http.HttpClient(handler) { BaseAddress = new Uri(apiBaseAddress) });
#else
            // Production / Release - use real API URL
            builder.Services.AddSingleton(new System.Net.Http.HttpClient { BaseAddress = new Uri("https://your-production-api/") });
#endif
            builder.Services.AddSingleton<IAuthService, AuthService>();
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<RegisterViewModel>();
            builder.Services.AddTransient<DashboardViewModel>();
            builder.Services.AddTransient<Views.LoginPage>();
            builder.Services.AddTransient<Views.RegisterPage>();
            builder.Services.AddTransient<Views.DashboardPage>();

            var app = builder.Build();

            // expose the service provider for pages/viewmodels
            App.Services = app.Services;

            return app;
        }
    }
}
