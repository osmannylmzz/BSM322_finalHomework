using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using EntityLayer.Models;

namespace DataLayer.Weather
{
    public static class WeatherDL
    {
        private static readonly HttpClient _httpClient = new();

        // wttr.in API - JSON formatında hava durumu
        private const string ApiBaseUrl = "https://wttr.in/";

        /// <summary>
        /// Belirtilen şehir için hava durumu verisini çeker
        /// </summary>
        public static async Task<CityWeather> GetWeatherAsync(string cityName)
        {
            var normalizedCity = NormalizeTurkishChars(cityName);
            var url = $"{ApiBaseUrl}{Uri.EscapeDataString(normalizedCity)}?format=j1&lang=tr";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            var weather = new CityWeather
            {
                CityName = cityName,
                Country = GetNestedValue(root, "nearest_area", "country"),
                CurrentTemp = GetCurrentWeatherValue(root, "temp_C") + "°C",
                CurrentCondition = GetCurrentWeatherValue(root, "lang_tr", true),
                Humidity = GetCurrentWeatherValue(root, "humidity") + "%",
                WindSpeed = GetCurrentWeatherValue(root, "windspeedKmph") + " km/s",
                FeelsLike = GetCurrentWeatherValue(root, "FeelsLikeC") + "°C",
                Forecasts = ParseForecasts(root)
            };

            return weather;
        }

        /// <summary>
        /// Türkçe karakterleri ASCII'ye dönüştürür
        /// </summary>
        public static string NormalizeTurkishChars(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            var replacements = new Dictionary<char, char>
            {
                { 'ç', 'c' }, { 'Ç', 'C' },
                { 'ğ', 'g' }, { 'Ğ', 'G' },
                { 'ı', 'i' }, { 'İ', 'I' },
                { 'ö', 'o' }, { 'Ö', 'O' },
                { 'ş', 's' }, { 'Ş', 'S' },
                { 'ü', 'u' }, { 'Ü', 'U' }
            };

            var result = new char[input.Length];
            for (int i = 0; i < input.Length; i++)
            {
                result[i] = replacements.TryGetValue(input[i], out var replacement)
                    ? replacement
                    : input[i];
            }

            return new string(result);
        }

        private static string GetCurrentWeatherValue(JsonElement root, string property, bool isLangArray = false)
        {
            try
            {
                if (root.TryGetProperty("current_condition", out var currentArr) &&
                    currentArr.GetArrayLength() > 0)
                {
                    var current = currentArr[0];

                    if (isLangArray)
                    {
                        // lang_tr dizisinden açıklama al
                        if (current.TryGetProperty("lang_tr", out var langArr) &&
                            langArr.GetArrayLength() > 0)
                        {
                            if (langArr[0].TryGetProperty("value", out var val))
                                return val.GetString() ?? "";
                        }
                        // Fallback: weatherDesc
                        if (current.TryGetProperty("weatherDesc", out var descArr) &&
                            descArr.GetArrayLength() > 0)
                        {
                            if (descArr[0].TryGetProperty("value", out var val))
                                return val.GetString() ?? "";
                        }
                    }
                    else
                    {
                        if (current.TryGetProperty(property, out var val))
                            return val.GetString() ?? "";
                    }
                }
            }
            catch { }
            return "-";
        }

        private static string GetNestedValue(JsonElement root, string arrayProp, string valueProp)
        {
            try
            {
                if (root.TryGetProperty(arrayProp, out var arr) && arr.GetArrayLength() > 0)
                {
                    var first = arr[0];
                    if (first.TryGetProperty(valueProp, out var valArr) && valArr.GetArrayLength() > 0)
                    {
                        if (valArr[0].TryGetProperty("value", out var val))
                            return val.GetString() ?? "";
                    }
                }
            }
            catch { }
            return "";
        }

        private static List<Forecast> ParseForecasts(JsonElement root)
        {
            var forecasts = new List<Forecast>();

            try
            {
                if (root.TryGetProperty("weather", out var weatherArr))
                {
                    foreach (var day in weatherArr.EnumerateArray())
                    {
                        var dateStr = day.TryGetProperty("date", out var d) ? d.GetString() ?? "" : "";
                        var dayName = "";

                        if (DateTime.TryParse(dateStr, out var date))
                        {
                            dayName = date.ToString("dddd", new CultureInfo("tr-TR"));
                            dateStr = date.ToString("dd MMM", new CultureInfo("tr-TR"));
                        }

                        var condition = "";
                        // hourly[0].lang_tr[0].value veya weatherDesc
                        if (day.TryGetProperty("hourly", out var hourlyArr) && hourlyArr.GetArrayLength() > 0)
                        {
                            var hourly = hourlyArr[0];
                            if (hourly.TryGetProperty("lang_tr", out var langArr) && langArr.GetArrayLength() > 0)
                            {
                                if (langArr[0].TryGetProperty("value", out var val))
                                    condition = val.GetString() ?? "";
                            }
                            if (string.IsNullOrEmpty(condition) &&
                                hourly.TryGetProperty("weatherDesc", out var descArr) && descArr.GetArrayLength() > 0)
                            {
                                if (descArr[0].TryGetProperty("value", out var val))
                                    condition = val.GetString() ?? "";
                            }
                        }

                        forecasts.Add(new Forecast
                        {
                            Date = dateStr,
                            DayName = dayName,
                            MaxTemp = (day.TryGetProperty("maxtempC", out var max) ? max.GetString() : "-") + "°C",
                            MinTemp = (day.TryGetProperty("mintempC", out var min) ? min.GetString() : "-") + "°C",
                            Condition = condition,
                            Icon = GetWeatherIcon(condition)
                        });
                    }
                }
            }
            catch { }

            return forecasts;
        }

        private static string GetWeatherIcon(string condition)
        {
            var lower = condition.ToLowerInvariant();

            if (lower.Contains("güneş") || lower.Contains("açık") || lower.Contains("sunny") || lower.Contains("clear"))
                return "☀️";
            if (lower.Contains("bulut") || lower.Contains("cloud") || lower.Contains("overcast"))
                return "☁️";
            if (lower.Contains("yağmur") || lower.Contains("rain") || lower.Contains("drizzle"))
                return "🌧️";
            if (lower.Contains("kar") || lower.Contains("snow"))
                return "❄️";
            if (lower.Contains("fırtına") || lower.Contains("thunder") || lower.Contains("storm"))
                return "⛈️";
            if (lower.Contains("sis") || lower.Contains("fog") || lower.Contains("mist"))
                return "🌫️";
            if (lower.Contains("parçalı") || lower.Contains("partly"))
                return "⛅";

            return "🌤️";
        }
    }
}