using CaseTrackerApp.Resources;
using CaseTrackerApp.ViewModels;
using Microsoft.Maui.Controls;

namespace CaseTrackerApp.Pages;

public class DashboardPage : ContentPage
{
    public DashboardPage(DashboardViewModel vm)
    {
        BindingContext = vm;
        Title = "Dashboard";
        var header = new Label { Text = "Advocate Dashboard", FontSize = 24, FontAttributes = FontAttributes.Bold };

        var statsGrid = new Grid { ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Star) }, ColumnSpacing = 10 };

        var activeCard = new Controls.StatCard();
        activeCard.SetBinding(Controls.StatCard.ValueProperty, nameof(vm.ActiveCases));
        activeCard.Title = "Active Cases";

        var todaysCard = new Controls.StatCard();
        todaysCard.SetBinding(Controls.StatCard.ValueProperty, nameof(vm.TodaysHearings));
        todaysCard.Title = "Today's Hearings";

        statsGrid.Add(activeCard, 0, 0);
        statsGrid.Add(todaysCard, 1, 0);

        var recent = new CollectionView { ItemTemplate = new DataTemplate(() => new Controls.CaseCard()) };
        recent.SetBinding(ItemsView.ItemsSourceProperty, nameof(vm.RecentCases));

        var layout = new VerticalStackLayout { Padding = 16, Spacing = 12, BackgroundColor = Theme.Background };
        layout.Children.Add(header);
        layout.Children.Add(statsGrid);
        layout.Children.Add(new Label { Text = "Recent Cases", FontAttributes = FontAttributes.Bold });
        layout.Children.Add(recent);

        Content = new ScrollView { Content = layout };

        this.Appearing += async (s, e) => await vm.LoadAsync();
    }
}
