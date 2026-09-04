using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
                    FilterHearings();
                }
            }
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
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

        #region Commands

        [RelayCommand]
        public async Task RefreshDashboardAsync()
        {
            if (IsBusy) return;

            IsBusy = true;
            try
            {
                await Task.Delay(800); // Smooth simulated refresh
                LoadDummyData();
            }
            finally
            {
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
