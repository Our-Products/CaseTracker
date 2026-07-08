using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CaseTracker.Shared.Dtos;
using System.Windows.Input;
using CaseTrackerApp.Services;
using System.Collections.ObjectModel;

namespace CaseTrackerApp.ViewModels;

public partial class CasesListViewModel : ViewModelBase
{
    private readonly ICaseApi _caseApi;

    public ObservableCollection<CaseDto> Cases { get; } = new();

    [ObservableProperty]
    private bool isBusy;

    public IAsyncRelayCommand LoadCommand { get; }

    public CasesListViewModel(ICaseApi caseApi)
    {
        _caseApi = caseApi;
        LoadCommand = new AsyncRelayCommand(async () => await LoadAsync());
    }

    public async Task LoadAsync()
    {
        if (IsBusy) return;
        try
        {
            IsBusy = true;
            Cases.Clear();
            var list = await _caseApi.GetCasesAsync();
            foreach (var c in list) Cases.Add(c);
        }
        finally
        {
            IsBusy = false;
        }
    }
}
