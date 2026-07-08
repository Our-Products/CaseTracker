using CaseTrackerApp.ViewModels;
using Microsoft.Maui.Controls;

namespace CaseTrackerApp.Pages;

public class ClientsPage : ContentPage
{
    public ClientsPage(ClientsViewModel vm)
    {
        BindingContext = vm;
        Title = "Clients";

        var collection = new CollectionView { ItemTemplate = new DataTemplate(() =>
        {
            var frame = new Frame { Padding = 10, CornerRadius = 8, BackgroundColor = Colors.White, Margin = new Thickness(8,4) };
            var name = new Label { FontAttributes = FontAttributes.Bold };
            name.SetBinding(Label.TextProperty, "Name");
            var email = new Label { TextColor = Colors.Gray };
            email.SetBinding(Label.TextProperty, "Email");
            frame.Content = new VerticalStackLayout { Children = { name, email } };
            return frame;
        }) };

        collection.SetBinding(ItemsView.ItemsSourceProperty, nameof(vm.Clients));

        Content = new ScrollView { Content = new VerticalStackLayout { Padding = 16, Children = { collection } } };
    }
}
