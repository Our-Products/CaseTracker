using CaseTrackerApp.ViewModels;
using Microsoft.Maui.Controls;

namespace CaseTrackerApp.Pages;

public class SettingsPage : ContentPage
{
    public SettingsPage(SettingsViewModel vm)
    {
        BindingContext = vm;
        Title = "Settings";

        var prefs = new VerticalStackLayout { Padding = 16, Spacing = 12 };
        prefs.Children.Add(new Label { Text = "Preferences", FontAttributes = FontAttributes.Bold });
        prefs.Children.Add(new Label { Text = "(Demo settings will appear here)" , TextColor = Colors.Gray});

        Content = new ScrollView { Content = prefs };
    }
}
