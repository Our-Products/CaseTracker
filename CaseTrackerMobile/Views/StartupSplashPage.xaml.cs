using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

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
                await InitializeVideoAndStartSequenceAsync();
            }
        }

        private async Task InitializeVideoAndStartSequenceAsync()
        {
            try
            {
                // Load MP4 video asset from Resources/Raw/startup_loader.mp4
                using var stream = await FileSystem.OpenAppPackageFileAsync("startup_loader.mp4");
                using var ms = new MemoryStream();
                await stream.CopyToAsync(ms);
                byte[] videoBytes = ms.ToArray();
                string base64Video = Convert.ToBase64String(videoBytes);

                string htmlContent = $@"<!DOCTYPE html>
<html>
<head>
<meta name='viewport' content='width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no'>
<style>
  html, body {{
    margin: 0;
    padding: 0;
    width: 100%;
    height: 100%;
    background-color: #0D1117;
    display: flex;
    justify-content: center;
    align-items: center;
    overflow: hidden;
  }}
  video {{
    width: 100%;
    height: 100%;
    object-fit: cover;
    border-radius: 18px;
  }}
</style>
</head>
<body>
  <video autoplay loop muted playsinline src='data:video/mp4;base64,{base64Video}'></video>
</body>
</html>";

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (LoaderVideoView != null)
                    {
                        LoaderVideoView.Source = new HtmlWebViewSource { Html = htmlContent };
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Startup video notice: {ex.Message}");
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (FallbackLogoView != null)
                    {
                        FallbackLogoView.IsVisible = true;
                    }
                });
            }

            // Pulsing animation trigger
            _ = Task.Run(async () =>
            {
                try
                {
                    if (ScalesLogoBorder != null)
                    {
                        await ScalesLogoBorder.ScaleToAsync(1.08, 600, Easing.CubicOut);
                        await ScalesLogoBorder.ScaleToAsync(1.0, 600, Easing.CubicIn);
                    }
                }
                catch { }
            });

            // Allow video loader to play for a smooth intro
            await Task.Delay(2200);

            if (StatusLabel != null)
            {
                StatusLabel.Text = "Workspace Ready!";
            }

            await Task.Delay(300);

            // Safely navigate to Login page via Shell
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    // Unhook WebView source before unmounting to prevent Android WebView window detachment crash
                    if (LoaderVideoView != null)
                    {
                        LoaderVideoView.Source = null;
                    }

                    if (Shell.Current != null)
                    {
                        await Shell.Current.GoToAsync("//Login");
                    }
                    else if (Application.Current != null && Application.Current.Windows.Count > 0)
                    {
                        Application.Current.Windows[0].Page = new AppShell();
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Navigation error: {ex.Message}");
                }
            });
        }
    }
}
