namespace CaseTrackerMobile
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("Login", typeof(Views.LoginPage));
            Routing.RegisterRoute("Register", typeof(Views.RegisterPage));
            Routing.RegisterRoute("Dashboard", typeof(Views.DashboardPage));
        }
    }
}
