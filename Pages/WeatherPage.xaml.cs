using System;
using System.Collections.Generic;
using BusinessLayer.Weather;
using EntityLayer.Models;

namespace finalHomework.Pages
{
    public partial class WeatherPage : ContentPage
    {
        private List<SavedCity> _cities = new();

        public WeatherPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadCitiesAsync();
        }

        private async Task LoadCitiesAsync()
        {
            try
            {
                _cities = await WeatherBL.LoadCitiesAsync();
                RefreshCityPicker();

                // Ýlk þehri seç
                if (_cities.Count > 0 && CityPicker.SelectedIndex < 0)
                {
                    CityPicker.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hata", $"Þehirler yüklenemedi:\n{ex.Message}", "Tamam");
            }
        }

        private void RefreshCityPicker()
        {
            CityPicker.Items.Clear();
            foreach (var city in _cities)
            {
                CityPicker.Items.Add(city.Name);
            }
        }

        private async void OnCityChanged(object sender, EventArgs e)
        {
            if (CityPicker.SelectedIndex < 0)
                return;

            var selectedCity = _cities[CityPicker.SelectedIndex];
            await LoadWeatherAsync(selectedCity.Name);
        }

        private async Task LoadWeatherAsync(string cityName)
        {
            try
            {
                LoadingOverlay.IsVisible = true;
                WeatherContent.IsVisible = false;

                var weather = await WeatherBL.GetWeatherAsync(cityName);

                // UI güncelle
                CityNameLabel.Text = weather.CityName;
                CurrentTempLabel.Text = weather.CurrentTemp;
                ConditionLabel.Text = weather.CurrentCondition;
                HumidityLabel.Text = weather.Humidity;
                WindLabel.Text = weather.WindSpeed;
                FeelsLikeLabel.Text = weather.FeelsLike;

                ForecastCollectionView.ItemsSource = weather.Forecasts;

                WeatherContent.IsVisible = true;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hata", $"Hava durumu alýnamadý:\n{ex.Message}", "Tamam");
            }
            finally
            {
                LoadingOverlay.IsVisible = false;
            }
        }

        private async void OnAddCityClicked(object sender, EventArgs e)
        {
            try
            {
                var cityName = CityEntry.Text?.Trim();

                if (string.IsNullOrWhiteSpace(cityName))
                {
                    await DisplayAlert("Uyarý", "Lütfen þehir adý girin.", "Tamam");
                    return;
                }

                LoadingOverlay.IsVisible = true;

                // Önce þehrin geçerli olup olmadýðýný kontrol et
                try
                {
                    await WeatherBL.GetWeatherAsync(cityName);
                }
                catch
                {
                    await DisplayAlert("Hata", "Þehir bulunamadý. Lütfen geçerli bir þehir adý girin.", "Tamam");
                    LoadingOverlay.IsVisible = false;
                    return;
                }

                // Þehri ekle
                _cities = await WeatherBL.AddCityAsync(cityName);
                RefreshCityPicker();

                // Yeni eklenen þehri seç
                var index = _cities.FindIndex(c => c.Name.Equals(cityName, StringComparison.OrdinalIgnoreCase));
                if (index >= 0)
                {
                    CityPicker.SelectedIndex = index;
                }

                CityEntry.Text = "";

                await DisplayAlert("Baþarýlý", $"{cityName} eklendi.", "Tamam");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hata", ex.Message, "Tamam");
            }
            finally
            {
                LoadingOverlay.IsVisible = false;
            }
        }

        private async void OnDeleteCityClicked(object sender, EventArgs e)
        {
            try
            {
                if (CityPicker.SelectedIndex < 0)
                {
                    await DisplayAlert("Uyarý", "Lütfen silinecek þehri seçin.", "Tamam");
                    return;
                }

                var selectedCity = _cities[CityPicker.SelectedIndex];

                var confirm = await DisplayAlert("Onay",
                    $"{selectedCity.Name} silinecek. Emin misiniz?",
                    "Evet", "Hayýr");

                if (!confirm)
                    return;

                _cities = await WeatherBL.RemoveCityAsync(selectedCity.Name);
                RefreshCityPicker();

                WeatherContent.IsVisible = false;

                // Baþka þehir varsa onu seç
                if (_cities.Count > 0)
                {
                    CityPicker.SelectedIndex = 0;
                }

                await DisplayAlert("Baþarýlý", $"{selectedCity.Name} silindi.", "Tamam");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hata", ex.Message, "Tamam");
            }
        }
    }
}