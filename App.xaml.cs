using DataLayer.Storage;

namespace finalHomework
{
    public partial class App : Application
    {
        private const string ThemePreferenceKey = "AppTheme";

        public App()
        {
            InitializeComponent();

            // LocalStorageDL'yi initialize et
            LocalStorageDL.Initialize(FileSystem.AppDataDirectory);

            // Kayıtlı temayı uygula
            ApplySavedTheme();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new NavigationPage(new Pages.LoginPage()));
        }

        /// <summary>
        /// Kayıtlı tema tercihini uygular
        /// </summary>
        private void ApplySavedTheme()
        {
            var isDarkMode = Preferences.Get(ThemePreferenceKey, false);
            UserAppTheme = isDarkMode ? AppTheme.Dark : AppTheme.Light;
        }
    }
}