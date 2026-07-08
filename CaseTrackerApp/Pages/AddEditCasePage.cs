using CaseTrackerApp.ViewModels;
using Microsoft.Maui.Controls;

namespace CaseTrackerApp.Pages;

public class AddEditCasePage : ContentPage
{
    public AddEditCasePage(AddEditCaseViewModel vm)
    {
        BindingContext = vm;
        Title = "Add / Edit Case";

        var caseNumber = new Entry { Placeholder = "Case Number" };
        caseNumber.SetBinding(Entry.TextProperty, nameof(vm.CaseNumber));
        var title = new Entry { Placeholder = "Title" };
        title.SetBinding(Entry.TextProperty, nameof(vm.Title));
        var opponent = new Entry { Placeholder = "Opponent" };
        opponent.SetBinding(Entry.TextProperty, nameof(vm.Opponent));
        var description = new Editor { Placeholder = "Description", HeightRequest = 120 };
        description.SetBinding(Editor.TextProperty, nameof(vm.Description));

        var save = new Button { Text = "Save", BackgroundColor = Colors.Green, TextColor = Colors.White };
        save.SetBinding(Button.CommandProperty, nameof(vm.SaveCommand));

        Content = new ScrollView { Content = new VerticalStackLayout { Padding = 16, Spacing = 12, Children = { caseNumber, title, opponent, description, save } } };
    }
}
