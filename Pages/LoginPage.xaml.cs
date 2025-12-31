using System;
using BusinessLayer.Auth;
using finalHomework.Services;

namespace finalHomework.Pages
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            try
            {
                var email = EmailEntry.Text?.Trim() ?? "";
                var pass = PasswordEntry.Text ?? "";

                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(pass))
                {
                    await DisplayAlert("Hata", "Email ve þifre boþ olamaz.", "Tamam");
                    return;
                }

                var (ok, error, uid) = await AuthBL.LoginAsync(email, pass);
                if (!ok)
                {
                    await DisplayAlert("Hata", string.IsNullOrWhiteSpace(error) ? "Giriþ baþarýsýz." : error, "Tamam");
                    return;
                }

                if (string.IsNullOrWhiteSpace(uid))
                {
                    await DisplayAlert("Hata", "Giriþ baþarýlý ama uid alýnamadý.", "Tamam");
                    return;
                }

                Session.CurrentUid = uid;
                Application.Current!.Windows[0].Page = new AppShell();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hata", ex.Message, "Tamam");
            }
        }

        private async void OnGoRegisterClicked(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushAsync(new RegisterPage());
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hata", ex.Message, "Tamam");
            }
        }
    }
}
