using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using EntityLayer.Models;

namespace DataLayer.Currency
{
    public static class CurrencyDL
    {
        private static readonly HttpClient _httpClient = new();

        // API URL'leri
        private const string GenelParaApiUrl = "https://api.genelpara.com/embed/altin.json";
        private const string TruncgilApiUrl = "https://finans.truncgil.com/today.json";

        
        /// API'den döviz/altýn kurlarýný çeker ve liste olarak döndürür.
        /// Önce GenelPara, baþarýsýz olursa Truncgil API'sini dener.
        
        public static async Task<List<EntityLayer.Models.Currency>> GetCurrenciesAsync()
        {
            // Önce GenelPara API'sini dene
            try
            {
                var result = await TryGenelParaApiAsync();
                if (result.Count > 0)
                    return result;
            }
            catch { /* GenelPara baþarýsýz, Truncgil'i dene */ }

            // Truncgil API'sini dene
            try
            {
                var result = await TryTruncgilApiAsync();
                if (result.Count > 0)
                    return result;
            }
            catch { /* Truncgil de baþarýsýz */ }

            // Her iki API de baþarýsýz olursa boþ liste döndür
            throw new Exception("Kur verileri alýnamadý. Lütfen internet baðlantýnýzý kontrol edin.");
        }

       
        /// GenelPara API - Format: { "USD": { "satis": "...", "alis": "...", "degisim": "..." }, ... }
  
        private static async Task<List<EntityLayer.Models.Currency>> TryGenelParaApiAsync()
        {
            var result = new List<EntityLayer.Models.Currency>();

            var response = await _httpClient.GetAsync(GenelParaApiUrl);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            foreach (var prop in root.EnumerateObject())
            {
                var code = prop.Name;

                if (prop.Value.ValueKind != JsonValueKind.Object)
                    continue;

                var obj = prop.Value;

                // GenelPara küçük harfli property kullanýyor
                var currency = new EntityLayer.Models.Currency
                {
                    Code = code,
                    Name = GetDisplayName(code),
                    Buying = GetPropertyValue(obj, "alis"),
                    Selling = GetPropertyValue(obj, "satis"),
                    Change = GetPropertyValue(obj, "degisim")
                };

                if (currency.Buying != "-" || currency.Selling != "-")
                    result.Add(currency);
            }

            return result;
        }


        /// Truncgil API - Format: { "Update_Date": "...", "USD": { "Piyes": "...", "Alýþ": "...", "Satýþ": "...", "Deðiþim": "..." }, ... }
    
        private static async Task<List<EntityLayer.Models.Currency>> TryTruncgilApiAsync()
        {
            var result = new List<EntityLayer.Models.Currency>();

            var response = await _httpClient.GetAsync(TruncgilApiUrl);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            foreach (var prop in root.EnumerateObject())
            {
                var code = prop.Name;

                // Meta alanlarý atla
                if (code == "Update_Date" || prop.Value.ValueKind != JsonValueKind.Object)
                    continue;

                var obj = prop.Value;

                string buying = "-", selling = "-", change = "-";

                // Tüm property'leri tara (Türkçe karakter sorunu için)
                foreach (var innerProp in obj.EnumerateObject())
                {
                    var propName = innerProp.Name;
                    var propValue = innerProp.Value.ToString();

                    // Alýþ kontrolü
                    if (ContainsAny(propName, "Alýþ", "Alis", "alis", "alýþ"))
                        buying = propValue;
                    // Satýþ kontrolü  
                    else if (ContainsAny(propName, "Satýþ", "Satis", "satis", "satýþ"))
                        selling = propValue;
                    // Deðiþim kontrolü
                    else if (ContainsAny(propName, "Deðiþim", "Degisim", "degisim", "deðiþim"))
                        change = propValue;
                }

                if (buying != "-" || selling != "-")
                {
                    result.Add(new EntityLayer.Models.Currency
                    {
                        Code = code,
                        Name = GetDisplayName(code),
                        Buying = buying,
                        Selling = selling,
                        Change = change
                    });
                }
            }

            return result;
        }

        private static bool ContainsAny(string source, params string[] values)
        {
            foreach (var val in values)
            {
                if (source.Equals(val, StringComparison.OrdinalIgnoreCase) ||
                    source.Contains(val, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        private static string GetPropertyValue(JsonElement element, string propertyName)
        {
            if (element.TryGetProperty(propertyName, out var value))
            {
                var str = value.ToString();
                if (!string.IsNullOrWhiteSpace(str))
                    return str;
            }
            return "-";
        }

        /// <summary>
        /// Kur koduna göre okunabilir isim döndürür.
        /// </summary>
        private static string GetDisplayName(string code)
        {
            return code switch
            {
                "USD" => "Amerikan Dolarý",
                "EUR" => "Euro",
                "GBP" => "Ýngiliz Sterlini",
                "GA" => "Gram Altýn",
                "C" => "Cumhuriyet Altýný",
                "Q" => "Çeyrek Altýn",
                "Y" => "Yarým Altýn",
                "T" => "Tam Altýn",
                "ATA" => "Ata Altýn",
                "gram-altin" => "Gram Altýn",
                "ceyrek-altin" => "Çeyrek Altýn",
                "yarim-altin" => "Yarým Altýn",
                "tam-altin" => "Tam Altýn",
                "cumhuriyet-altini" => "Cumhuriyet Altýný",
                "ata-altin" => "Ata Altýn",
                "22-ayar-bilezik" => "22 Ayar Bilezik",
                "CHF" => "Ýsviçre Frangý",
                "CAD" => "Kanada Dolarý",
                "RUB" => "Rus Rublesi",
                "AED" => "BAE Dirhemi",
                "AUD" => "Avustralya Dolarý",
                "DKK" => "Danimarka Kronu",
                "SEK" => "Ýsveç Kronu",
                "NOK" => "Norveç Kronu",
                "JPY" => "Japon Yeni",
                "SAR" => "Suudi Riyali",
                "KWD" => "Kuveyt Dinarý",
                "gumus" => "Gümüþ",
                "ONS" => "Ons Altýn",
                "BTC" => "Bitcoin",
                "ETH" => "Ethereum",
                "XU100" => "BIST 100",
                _ => code.Replace("-", " ")
            };
        }
    }
}