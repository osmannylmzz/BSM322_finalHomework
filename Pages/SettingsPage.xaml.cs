using System;
using finalHomework.Services;
using Microsoft.Maui.Controls;

namespace finalHomework.Pages
{
    public partial class SettingsPage : ContentPage
    {
        private const string ThemePreferenceKey = "AppTheme";

        public SettingsPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadThemePreference();
        }

        /// <summary>
        /// Kayýtlý tema tercihini yükler
        /// </summary>
        private void LoadThemePreference()
        {
            var isDarkMode = Preferences.Get(ThemePreferenceKey, false);
            DarkModeSwitch.IsToggled = isDarkMode;
            UpdateThemePreview(isDarkMode);
        }

        /// <summary>
        /// Tema switch deðiþtiðinde
        /// </summary>
        private void OnDarkModeToggled(object sender, ToggledEventArgs e)
        {
            var isDarkMode = e.Value;

            // Tercihi kaydet
            Preferences.Set(ThemePreferenceKey, isDarkMode);

            // Temayý uygula
            ApplyTheme(isDarkMode);

            // Önizlemeyi güncelle
            UpdateThemePreview(isDarkMode);
        }

        /// <summary>
        /// Temayý uygulamaya uygular
        /// </summary>
        private void ApplyTheme(bool isDarkMode)
        {
            if (Application.Current != null)
            {
                Application.Current.UserAppTheme = isDarkMode
                    ? AppTheme.Dark
                    : AppTheme.Light;
            }
        }

        /// <summary>
        /// Tema önizlemesini günceller
        /// </summary>
        private void UpdateThemePreview(bool isDarkMode)
        {
            if (isDarkMode)
            {
                ThemePreviewFrame.BackgroundColor = Color.FromArgb("#1E1E1E");
                ThemePreviewTitle.TextColor = Colors.White;
                ThemePreviewText.TextColor = Color.FromArgb("#CCCCCC");
            }
            else
            {
                ThemePreviewFrame.BackgroundColor = Color.FromArgb("#F5F5F5");
                ThemePreviewTitle.TextColor = Colors.Black;
                ThemePreviewText.TextColor = Color.FromArgb("#333333");
            }
        }

        /// <summary>
        /// Çýkýþ yap
        /// </summary>
        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            var confirm = await DisplayAlert("Çýkýþ",
                "Çýkýþ yapmak istediðinize emin misiniz?",
                "Evet", "Hayýr");

            if (!confirm)
                return;

            // Session temizle
            Session.CurrentUid = null;

            // Login sayfasýna dön
            if (Application.Current != null)
            {
                Application.Current.Windows[0].Page = new NavigationPage(new LoginPage());
            }
        }
    }
}