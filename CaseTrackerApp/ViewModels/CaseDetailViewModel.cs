using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CaseTracker.Shared.Dtos;
using CaseTrackerApp.Services;

namespace CaseTrackerApp.ViewModels;

public partial class CaseDetailViewModel : ViewModelBase
{
    private readonly ICaseApi _caseApi;

    [ObservableProperty]
    private CaseDto? selectedCase;

    public IAsyncRelayCommand<int> LoadCommand { get; }
    public IAsyncRelayCommand DeleteCommand { get; }

    public CaseDetailViewModel(ICaseApi caseApi)
    {
        _caseApi = caseApi;
        LoadCommand = new AsyncRelayCommand<int>(async id => await LoadAsync(id));
        DeleteCommand = new AsyncRelayCommand(async () => await DeleteAsync());
    }

    /// <summary>
    /// Public method to load a case by id so pages can call it.
    /// </summary>
    public async Task LoadAsync(int id)
    {
        SelectedCase = await _caseApi.GetCaseAsync(id);
    }

    /// <summary>
    /// Public method to delete the selected case so pages can call it.
    /// </summary>
    public async Task DeleteAsync()
    {
        if (SelectedCase == null) return;
        await _caseApi.DeleteCaseAsync(SelectedCase.Id);
    }
}
