using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CaseTracker.Shared.Dtos;
using CaseTrackerApp.Services;
using Microsoft.Maui.Controls;

namespace CaseTrackerApp.ViewModels;

public partial class AddEditCaseViewModel : ViewModelBase
{
    private readonly ICaseApi _caseApi;

    [ObservableProperty]
    private string caseNumber = string.Empty;

    [ObservableProperty]
    private string title = string.Empty;

    [ObservableProperty]
    private string opponent = string.Empty;

    [ObservableProperty]
    private string description = string.Empty;

    public IAsyncRelayCommand SaveCommand { get; }

    public AddEditCaseViewModel(ICaseApi caseApi)
    {
        _caseApi = caseApi;
        SaveCommand = new AsyncRelayCommand(async () => await SaveAsync());
    }

    private async Task SaveAsync()
    {
        var item = new CaseDto { Id = 0, CaseNumber = CaseNumber, Title = Title, Opponent = Opponent, Description = Description };
        await _caseApi.CreateCaseAsync(item);
        await Shell.Current.GoToAsync("..", true);
    }
}
