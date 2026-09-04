using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;

namespace CaseTrackerMobile.ViewModels
{
    public partial class DashboardHearingItem : ObservableObject
    {
        public string CaseNumber { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string CourtName { get; set; } = string.Empty;
        public string Stage { get; set; } = string.Empty;
        public string ItemNumber { get; set; } = string.Empty;
        public string BadgeText { get; set; } = string.Empty;
        public string BadgeVariant { get; set; } = "default"; // default, secondary, outline, destructive
    }

    public partial class DashboardViewModel : ObservableObject
    {
        private CancellationTokenSource? _searchCts;
        private CancellationTokenSource? _loadingDelayCts;

        #region Properties

        private string _advocateName = "Adv. R. Sundaram";
        public string AdvocateName
        {
            get => _advocateName;
            set => SetProperty(ref _advocateName, value);
        }

        private string _barCouncilId = "TN/1042/2018";
        public string BarCouncilId
        {
            get => _barCouncilId;
            set => SetProperty(ref _barCouncilId, value);
        }

        private string _lawFirmName = "Sundaram & Associates";
        public string LawFirmName
        {
            get => _lawFirmName;
            set => SetProperty(ref _lawFirmName, value);
        }

        private string _activeCasesCount = "42";
        public string ActiveCasesCount
        {
            get => _activeCasesCount;
            set => SetProperty(ref _activeCasesCount, value);
        }

        private string _todaysHearingsCount = "05";
        public string TodaysHearingsCount
        {
            get => _todaysHearingsCount;
            set => SetProperty(ref _todaysHearingsCount, value);
        }

        private string _pendingOrdersCount = "08";
        public string PendingOrdersCount
        {
            get => _pendingOrdersCount;
            set => SetProperty(ref _pendingOrdersCount, value);
        }

        private string _totalClientsCount = "128";
        public string TotalClientsCount
        {
            get => _totalClientsCount;
            set => SetProperty(ref _totalClientsCount, value);
        }

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    _ = TriggerDebouncedSearchAsync();
                }
            }
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public ObservableCollection<DashboardHearingItem> Hearings { get; } = new();
        private readonly ObservableCollection<DashboardHearingItem> _allHearings = new();

        #endregion

        public DashboardViewModel()
        {
            LoadDummyData();
        }

        private void LoadDummyData()
        {
            _allHearings.Clear();

            _allHearings.Add(new DashboardHearingItem
            {
                CaseNumber = "OS/412/2024",
                Title = "K. Ramanathan vs. State of Tamil Nadu",
                CourtName = "Madras High Court - Hall No. 4",
                Stage = "Cross Examination of PW-1",
                ItemNumber = "Item #04",
                BadgeText = "Urgent Hearing",
                BadgeVariant = "destructive"
            });

            _allHearings.Add(new DashboardHearingItem
            {
                CaseNumber = "WP/18920/2023",
                Title = "M/s Apex Logistics Pvt Ltd vs. Port Trust",
                CourtName = "High Court Bench - Hall No. 12",
                Stage = "Final Arguments & Written Submissions",
                ItemNumber = "Item #12",
                BadgeText = "Listed",
                BadgeVariant = "default"
            });

            _allHearings.Add(new DashboardHearingItem
            {
                CaseNumber = "CMA/88/2024",
                Title = "S. Meenakshi vs. K. Sundaram & Ors",
                CourtName = "City Civil Court - Court Room 2",
                Stage = "Interim Injunction Application",
                ItemNumber = "Item #18",
                BadgeText = "Passed Over",
                BadgeVariant = "secondary"
            });

            _allHearings.Add(new DashboardHearingItem
            {
                CaseNumber = "CC/1052/2023",
                Title = "V. Anand vs. City Municipal Corporation",
                CourtName = "Principal Sessions Court - Room 5",
                Stage = "Framing of Charges",
                ItemNumber = "Item #25",
                BadgeText = "Awaiting Order",
                BadgeVariant = "outline"
            });

            _allHearings.Add(new DashboardHearingItem
            {
                CaseNumber = "OP/304/2024",
                Title = "T. Rajesh vs. Union of India & Anr",
                CourtName = "Debts Recovery Tribunal - Bench 1",
                Stage = "Admission & Notice Returnable",
                ItemNumber = "Item #31",
                BadgeText = "Listed",
                BadgeVariant = "default"
            });

            FilterHearings();
        }

        private void FilterHearings()
        {
            Hearings.Clear();

            var query = _allHearings.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var term = SearchText.Trim().ToLowerInvariant();
                query = query.Where(h =>
                    h.CaseNumber.ToLowerInvariant().Contains(term) ||
                    h.Title.ToLowerInvariant().Contains(term) ||
                    h.CourtName.ToLowerInvariant().Contains(term) ||
                    h.Stage.ToLowerInvariant().Contains(term));
            }

            foreach (var item in query)
            {
                Hearings.Add(item);
            }
        }

        #region Commands & 200ms Debounced Data-Fetching Strategy

        [RelayCommand]
        public async Task TriggerDebouncedSearchAsync()
        {
            // Cancel previous active search task
            _searchCts?.Cancel();
            _searchCts = new CancellationTokenSource();
            var token = _searchCts.Token;

            _loadingDelayCts?.Cancel();
            _loadingDelayCts = new CancellationTokenSource();
            var delayToken = _loadingDelayCts.Token;

            // 1. Debounce user keystrokes by 200ms
            try
            {
                await Task.Delay(200, token);
            }
            catch (TaskCanceledException)
            {
                return;
            }

            // 2. Start 200ms delay timer before setting IsLoading = true
            // If data fetching finishes in <200ms, timer is cancelled and IsLoading stays false (zero UI flicker)
            var showLoadingTask = Task.Delay(200, delayToken).ContinueWith(t =>
            {
                if (!t.IsCanceled && !token.IsCancellationRequested)
                {
                    MainThread.BeginInvokeOnMainThread(() => IsLoading = true);
                }
            }, TaskScheduler.Default);

            try
            {
                // Simulated async network data fetch delay
                await Task.Delay(250, token);
                FilterHearings();
            }
            catch (TaskCanceledException)
            {
                // Search operation cancelled by subsequent keystroke
            }
            finally
            {
                // Cancel loading delay timer if work completed fast (<200ms)
                _loadingDelayCts?.Cancel();
                MainThread.BeginInvokeOnMainThread(() => IsLoading = false);
            }
        }

        [RelayCommand]
        public async Task RefreshDashboardAsync()
        {
            if (IsBusy) return;

            IsBusy = true;

            _loadingDelayCts?.Cancel();
            _loadingDelayCts = new CancellationTokenSource();
            var delayToken = _loadingDelayCts.Token;

            // 200ms debounced loader display
            var showLoadingTask = Task.Delay(200, delayToken).ContinueWith(t =>
            {
                if (!t.IsCanceled)
                {
                    MainThread.BeginInvokeOnMainThread(() => IsLoading = true);
                }
            }, TaskScheduler.Default);

            try
            {
                await Task.Delay(500); // Async data fetching simulation
                LoadDummyData();
            }
            finally
            {
                _loadingDelayCts?.Cancel();
                MainThread.BeginInvokeOnMainThread(() => IsLoading = false);
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task AddNewCaseAsync()
        {
            await ShowAlertAsync("New Case Registration", "Feature to register new case file in EF Core database is starting.");
        }

        [RelayCommand]
        public async Task SearchCauseListAsync()
        {
            await ShowAlertAsync("Live Cause List", "Fetching real-time cause list from eCourts integration.");
        }

        [RelayCommand]
        public async Task LogoutAsync()
        {
            SecureStorage.Default.Remove("auth_token");
            if (Shell.Current != null)
            {
                await Shell.Current.GoToAsync("//Login");
            }
        }

        private async Task ShowAlertAsync(string title, string message)
        {
            if (Shell.Current != null)
                await Shell.Current.DisplayAlert(title, message, "OK");
            else if (Application.Current?.Windows.Count > 0 && Application.Current.Windows[0].Page != null)
                await Application.Current.Windows[0].Page!.DisplayAlert(title, message, "OK");
        }

        #endregion
    }
}
