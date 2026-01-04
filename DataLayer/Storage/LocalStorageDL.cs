using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using EntityLayer.Models;

namespace DataLayer.Storage
{
    public static class LocalStorageDL
    {
        private const string CitiesFileName = "cities.json";

        // UI tarafýndan set edilecek
        private static string? _appDataDirectory;

     
        /// UI projesinden AppDataDirectory set edilir
       
        public static void Initialize(string appDataDirectory)
        {
            _appDataDirectory = appDataDirectory;
        }

        private static string GetFilePath()
        {
            if (string.IsNullOrWhiteSpace(_appDataDirectory))
                throw new InvalidOperationException("LocalStorageDL.Initialize() çaðrýlmadý. Önce UI'dan Initialize edin.");

            return Path.Combine(_appDataDirectory, CitiesFileName);
        }

     
        /// Kaydedilmiþ þehirleri okur
 
        public static async Task<List<SavedCity>> LoadCitiesAsync()
        {
            try
            {
                var filePath = GetFilePath();

                if (!File.Exists(filePath))
                    return GetDefaultCities();

                var json = await File.ReadAllTextAsync(filePath);

                if (string.IsNullOrWhiteSpace(json))
                    return GetDefaultCities();

                var cities = JsonSerializer.Deserialize<List<SavedCity>>(json);
                return cities ?? GetDefaultCities();
            }
            catch
            {
                return GetDefaultCities();
            }
        }

     
        /// Þehirleri JSON olarak kaydeder
       
        public static async Task SaveCitiesAsync(List<SavedCity> cities)
        {
            try
            {
                var filePath = GetFilePath();
                var json = JsonSerializer.Serialize(cities, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                await File.WriteAllTextAsync(filePath, json);
            }
            catch (Exception ex)
            {
                throw new Exception($"Þehirler kaydedilemedi: {ex.Message}");
            }
        }

 
        /// Varsayýlan þehir listesi
 
        private static List<SavedCity> GetDefaultCities()
        {
            return new List<SavedCity>
            {
                new SavedCity { Name = "Ýstanbul", NormalizedName = "Istanbul" },
                new SavedCity { Name = "Ankara", NormalizedName = "Ankara" },
                new SavedCity { Name = "Ýzmir", NormalizedName = "Izmir" }
            };
        }
    }
}