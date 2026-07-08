using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Input = CommunityToolkit.Mvvm.Input;
using CaseTracker.Shared.Dtos;
using CaseTrackerApp.Services;
using System.Collections.ObjectModel;

namespace CaseTrackerApp.ViewModels;

public partial class DashboardViewModel : ViewModelBase
{
    private readonly ICaseApi _caseApi;

    public ObservableCollection<CaseDto> RecentCases { get; } = new();

    [ObservableProperty]
    private int activeCases;

    [ObservableProperty]
    private int todaysHearings;

    public IAsyncRelayCommand LoadCommand { get; }

    public DashboardViewModel(ICaseApi caseApi)
    {
        _caseApi = caseApi;
        LoadCommand = new AsyncRelayCommand(async () => await LoadAsync());
    }

    public async Task LoadAsync()
    {
        var list = await _caseApi.GetCasesAsync();
        RecentCases.Clear();
        foreach (var c in list.Take(5)) RecentCases.Add(c);
        ActiveCases = list.Count;
        TodaysHearings = list.Count(x => x.NextHearingDate?.Date == DateTime.Today);
    }
}
