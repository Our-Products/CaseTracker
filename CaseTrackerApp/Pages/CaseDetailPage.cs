using Microsoft.Maui.Controls;
using CaseTrackerApp.ViewModels;

namespace CaseTrackerApp.Pages;

[QueryProperty(nameof(CaseId), "caseId")]
public class CaseDetailPage : ContentPage
{
    public string CaseId { get; set; } = string.Empty;
    private readonly CaseDetailViewModel _vm;

    public CaseDetailPage(CaseDetailViewModel vm)
    {
        _vm = vm;
        BindingContext = _vm.SelectedCase;
        Title = "Case Detail";

        var caseNum = new Label { FontAttributes = FontAttributes.Bold, FontSize = 18 };
        caseNum.SetBinding(Label.TextProperty, "CaseNumber");
        var title = new Label();
        title.SetBinding(Label.TextProperty, "Title");
        var desc = new Label();
        desc.SetBinding(Label.TextProperty, "Description");
        var next = new Label { TextColor = Colors.Gray };
        next.SetBinding(Label.TextProperty, new Binding("NextHearingDate", stringFormat: "Next: {0:dd MMM yyyy}"));

        var deleteBtn = new Button { Text = "Delete", BackgroundColor = Colors.Red, TextColor = Colors.White };
        deleteBtn.Clicked += async (s, e) =>
        {
            await _vm.DeleteAsync();
            await Shell.Current.GoToAsync("..", true);
        };

        Content = new ScrollView
        {
            Content = new VerticalStackLayout { Padding = 16, Spacing = 12, Children = { caseNum, title, desc, next, deleteBtn } }
        };
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (int.TryParse(CaseId, out var id))
        {
            await _vm.LoadAsync(id);
        }
    }
}
