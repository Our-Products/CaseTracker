using CaseTrackerApp.ViewModels;
using Microsoft.Maui.Controls;

namespace CaseTrackerApp.Pages;

public class CasesListPage : ContentPage
{
    public CasesListPage(CasesListViewModel vm)
    {
        BindingContext = vm;
        Title = "Cases";
        // No-op patch: touched file to trigger rebuild

        var collection = new CollectionView { SelectionMode = SelectionMode.Single };
        collection.ItemTemplate = new DataTemplate(() => new Controls.CaseCard());

        collection.SetBinding(ItemsView.ItemsSourceProperty, nameof(vm.Cases));

            collection.SelectionChanged += async (s, e) =>
        {
            var item = e.CurrentSelection?.FirstOrDefault() as CaseTracker.Shared.Dtos.CaseDto;
            if (item != null)
            {
                await Shell.Current.GoToAsync($"{nameof(Pages.CaseDetailPage)}?caseId={item.Id}");
                ((CollectionView)s).SelectedItem = null;
            }
        };

        var refresh = new RefreshView { Content = collection };
        refresh.SetBinding(RefreshView.CommandProperty, nameof(vm.LoadCommand));
        refresh.SetBinding(RefreshView.IsRefreshingProperty, nameof(vm.IsBusy));

        Content = new StackLayout { Children = { refresh } };

        this.Appearing += async (s, e) => await vm.LoadAsync();
    }
}
