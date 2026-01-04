using System.Collections.Generic;
using System.Threading.Tasks;
using DataLayer.Storage;
using DataLayer.Weather;
using EntityLayer.Models;

namespace BusinessLayer.Weather
{
    public static class WeatherBL
    {
  
        /// Kaydedilmiþ þehirleri yükler

        public static Task<List<SavedCity>> LoadCitiesAsync()
            => LocalStorageDL.LoadCitiesAsync();


        /// Þehirleri kaydeder
     
        public static Task SaveCitiesAsync(List<SavedCity> cities)
            => LocalStorageDL.SaveCitiesAsync(cities);

    
        /// Yeni þehir ekler
 
        public static async Task<List<SavedCity>> AddCityAsync(string cityName)
        {
            var cities = await LoadCitiesAsync();

            var normalized = WeatherDL.NormalizeTurkishChars(cityName);

            // Zaten varsa ekleme
            if (cities.Exists(c => c.NormalizedName.Equals(normalized, System.StringComparison.OrdinalIgnoreCase)))
            {
                return cities;
            }

            cities.Add(new SavedCity
            {
                Name = cityName.Trim(),
                NormalizedName = normalized
            });

            await SaveCitiesAsync(cities);
            return cities;
        }

           /// Þehir siler
     
        public static async Task<List<SavedCity>> RemoveCityAsync(string cityName)
        {
            var cities = await LoadCitiesAsync();
            cities.RemoveAll(c => c.Name.Equals(cityName, System.StringComparison.OrdinalIgnoreCase));
            await SaveCitiesAsync(cities);
            return cities;
        }

     
        /// Hava durumu verisini çeker
   
        public static Task<CityWeather> GetWeatherAsync(string cityName)
            => WeatherDL.GetWeatherAsync(cityName);
    }
}