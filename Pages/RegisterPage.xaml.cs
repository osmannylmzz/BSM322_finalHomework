using System;
using BusinessLayer.Auth;

namespace finalHomework.Pages
{
    public partial class RegisterPage : ContentPage
    {
        public RegisterPage()
        {
            InitializeComponent();
        }

        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            try
            {
                var username = UsernameEntry.Text?.Trim() ?? "";
                var email = EmailEntry.Text?.Trim() ?? "";
                var pass = PasswordEntry.Text ?? "";

                if (string.IsNullOrWhiteSpace(username) ||
                    string.IsNullOrWhiteSpace(email) ||
                    string.IsNullOrWhiteSpace(pass))
                {
                    await DisplayAlert("Hata", "Tüm alanlarý doldurun.", "Tamam");
                    return;
                }

                var (ok, error, uid) = await AuthBL.RegisterAsync(username, email, pass);
                if (!ok)
                {
                    await DisplayAlert("Hata", string.IsNullOrWhiteSpace(error) ? "Kayýt baþarýsýz." : error, "Tamam");
                    return;
                }

                await DisplayAlert("OK", "Kayýt baþarýlý. Þimdi giriþ yapýn.", "Tamam");
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hata", ex.Message, "Tamam");
            }
        }
    }
}
