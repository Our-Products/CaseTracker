using CaseTrackerMobile.ViewModels;
using Microsoft.Maui.Controls;

namespace CaseTrackerMobile.Views;

public partial class DashboardPage : ContentPage
{
    public DashboardPage() : this(App.Services?.GetService(typeof(DashboardViewModel)) as DashboardViewModel ?? new DashboardViewModel())
    {
    }

    public DashboardPage(DashboardViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel ?? App.Services?.GetService(typeof(DashboardViewModel)) as DashboardViewModel ?? new DashboardViewModel();
    }
}
