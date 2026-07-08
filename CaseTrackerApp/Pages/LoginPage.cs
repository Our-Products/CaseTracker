using CaseTrackerApp.Services;
using CaseTrackerApp.ViewModels;
using Microsoft.Maui.Controls;

namespace CaseTrackerApp.Pages;

public class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel vm)
    {
        BindingContext = vm;
        Title = "Sign In";

        var user = new Entry { Placeholder = "Username" };
        user.SetBinding(Entry.TextProperty, nameof(vm.Username));
        var pass = new Entry { Placeholder = "Password", IsPassword = true };
        pass.SetBinding(Entry.TextProperty, nameof(vm.Password));
        var btn = new Button { Text = "Sign In", BackgroundColor = Colors.Blue, TextColor = Colors.White };
        btn.SetBinding(Button.CommandProperty, nameof(vm.SignInCommand));

        Content = new VerticalStackLayout { Padding = 16, Spacing = 12, Children = { user, pass, btn } };
    }
}
