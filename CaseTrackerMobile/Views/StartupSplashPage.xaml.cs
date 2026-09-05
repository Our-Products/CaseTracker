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
                System.Diagnostics.Debug.WriteLine($"Failed to load startup video: {ex.Message}");
            }

            // Allow video loader to play for a smooth branding intro
            await Task.Delay(2500);

            if (StatusLabel != null)
            {
                StatusLabel.Text = "Workspace Ready!";
            }

            await Task.Delay(400);

            // Safely transition to AppShell on MainThread
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
