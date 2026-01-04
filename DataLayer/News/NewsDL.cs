using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using EntityLayer.Models;

namespace DataLayer.News
{
    public static class NewsDL
    {
        private static readonly HttpClient _httpClient = new();

        // rss2json API base URL
        private const string Rss2JsonBase = "https://api.rss2json.com/v1/api.json?rss_url=";

 
        /// Tüm haber kategorilerini döndürür (TRT Haber RSS kaynaklarý)
  
        public static List<NewsCategory> GetCategories()
        {
            return new List<NewsCategory>
            {
                new NewsCategory { Name = "Manþet", RssUrl = "https://www.trthaber.com/manset_articles.rss" },
                new NewsCategory { Name = "Son Dakika", RssUrl = "https://www.trthaber.com/sondakika_articles.rss" },
                new NewsCategory { Name = "Gündem", RssUrl = "https://www.trthaber.com/gundem_articles.rss" },
                new NewsCategory { Name = "Ekonomi", RssUrl = "https://www.trthaber.com/ekonomi_articles.rss" },
                new NewsCategory { Name = "Spor", RssUrl = "https://www.trthaber.com/spor_articles.rss" },
                new NewsCategory { Name = "Bilim & Teknoloji", RssUrl = "https://www.trthaber.com/bilim_teknoloji_articles.rss" },
                new NewsCategory { Name = "Güncel", RssUrl = "https://www.trthaber.com/guncel_articles.rss" }
            };
        }

        
        /// Belirtilen RSS URL'den haberleri çeker (rss2json üzerinden)
    
        public static async Task<List<NewsItem>> GetNewsAsync(string rssUrl, string categoryName = "")
        {
            var result = new List<NewsItem>();

            // URL encode
            var encodedUrl = HttpUtility.UrlEncode(rssUrl);
            var apiUrl = Rss2JsonBase + encodedUrl;

            var response = await _httpClient.GetAsync(apiUrl);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            // Status kontrolü
            if (root.TryGetProperty("status", out var status) && status.GetString() != "ok")
            {
                throw new Exception("RSS feed alýnamadý.");
            }

            // items dizisini parse et
            if (root.TryGetProperty("items", out var items))
            {
                foreach (var item in items.EnumerateArray())
                {
                    var newsItem = new NewsItem
                    {
                        Title = GetStringValue(item, "title"),
                        Link = GetStringValue(item, "link"),
                        Description = CleanDescription(GetStringValue(item, "description")),
                        PubDate = FormatDate(GetStringValue(item, "pubDate")),
                        Thumbnail = GetThumbnail(item),
                        Category = categoryName
                    };

                    result.Add(newsItem);
                }
            }

            return result;
        }

        private static string GetStringValue(JsonElement element, string propertyName)
        {
            if (element.TryGetProperty(propertyName, out var value))
            {
                return value.GetString() ?? "";
            }
            return "";
        }

        
        /// Thumbnail URL'sini çeþitli alanlardan almaya çalýþýr
        
        private static string GetThumbnail(JsonElement item)
        {
            // enclosure içinde olabilir
            if (item.TryGetProperty("enclosure", out var enclosure))
            {
                if (enclosure.TryGetProperty("link", out var link))
                {
                    var url = link.GetString();
                    if (!string.IsNullOrWhiteSpace(url))
                        return url;
                }
            }

            // thumbnail alaný
            if (item.TryGetProperty("thumbnail", out var thumb))
            {
                var url = thumb.GetString();
                if (!string.IsNullOrWhiteSpace(url))
                    return url;
            }

            return "";
        }

       
        /// HTML tag'lerini temizler
       
        private static string CleanDescription(string html)
        {
            if (string.IsNullOrWhiteSpace(html))
                return "";

            // Basit HTML tag temizleme
            var result = System.Text.RegularExpressions.Regex.Replace(html, "<[^>]*>", "");
            result = HttpUtility.HtmlDecode(result);
            return result.Trim();
        }

        /// Tarih formatýný düzenler
       
        private static string FormatDate(string dateStr)
        {
            if (DateTime.TryParse(dateStr, out var date))
            {
                return date.ToString("dd MMM yyyy HH:mm");
            }
            return dateStr;
        }
    }
}