using System;
using Microsoft.Maui.Controls;
using CaseTrackerMobile.Views;

namespace CaseTrackerMobile
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            // Initialize with Dashboard view
            ContentArea.Content = new DashboardView();
        }

        private void OnDashboardClicked(object sender, EventArgs e)
        {
            ContentArea.Content = new DashboardView();
        }

        private void OnCasesClicked(object sender, EventArgs e)
        {
            ContentArea.Content = new CasesView();
        }

        private void OnClientsClicked(object sender, EventArgs e)
        {
            ContentArea.Content = new ClientsView();
        }

        private void OnDocumentsClicked(object sender, EventArgs e)
        {
            ContentArea.Content = new DocumentsView();
        }

        private void OnProfileClicked(object sender, EventArgs e)
        {
            ContentArea.Content = new ProfileView();
        }
    }
}
