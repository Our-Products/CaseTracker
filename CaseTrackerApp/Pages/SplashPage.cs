using CaseTrackerApp.ViewModels;
using Microsoft.Maui.Controls;

namespace CaseTrackerApp.Pages;

public class SplashPage : ContentPage
{
    public SplashPage(SplashViewModel vm)
    {
        BindingContext = vm;
        BackgroundColor = Colors.White;
        Content = new VerticalStackLayout
        {
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center,
            Children =
            {
                new Label { Text = "CaseTracker", FontSize = 32, FontAttributes = FontAttributes.Bold },
                new Label { Text = "Loading...", TextColor = Colors.Gray }
            }
        };
    }
}
