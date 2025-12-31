using System;
using System.Collections.ObjectModel;
using BusinessLayer.Currency;
using EntityLayer.Models;

namespace finalHomework.Pages
{
    public partial class CurrencyPage : ContentPage
    {
        private readonly ObservableCollection<Currency> _currencies = new();

        public CurrencyPage()
        {
            InitializeComponent();
            CurrencyCollectionView.ItemsSource = _currencies;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Sayfa açýlýnca ilk kez veri çek
            if (_currencies.Count == 0)
            {
                await LoadCurrenciesAsync();
            }
        }

        private async void OnRefreshClicked(object sender, EventArgs e)
        {
            await LoadCurrenciesAsync();
        }

        private async Task LoadCurrenciesAsync()
        {
            try
            {
                // Loading göster
                LoadingOverlay.IsVisible = true;
                RefreshButton.IsEnabled = false;

                // Veriyi çek
                var list = await CurrencyBL.GetCurrenciesAsync();

                // Listeyi güncelle
                _currencies.Clear();
                foreach (var c in list)
                {
                    _currencies.Add(c);
                }
            }
            catch (Exception ex)
            {
                // Hata durumunda kullanýcýya bildir
                await DisplayAlert("Hata", $"Kurlar yüklenirken hata oluþtu:\n{ex.Message}", "Tamam");
            }
            finally
            {
                // Loading gizle
                LoadingOverlay.IsVisible = false;
                RefreshButton.IsEnabled = true;
            }
        }
    }
}