namespace stockwatch
{
    public partial class App : Microsoft.Maui.Controls.Application
    {
        public App()
        {
            InitializeComponent();

            UserAppTheme = AppTheme.Light;
            MainPage = new AppShell();
        }
    }
}
