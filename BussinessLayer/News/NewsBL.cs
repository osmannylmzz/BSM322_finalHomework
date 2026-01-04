using System.Collections.Generic;
using System.Threading.Tasks;
using DataLayer.News;
using EntityLayer.Models;

namespace BusinessLayer.News
{
    public static class NewsBL
    {
   
        /// Haber kategorilerini döndürür
 
        public static List<NewsCategory> GetCategories()
            => NewsDL.GetCategories();


        /// Belirtilen RSS URL'den haberleri çeker
    
        public static Task<List<NewsItem>> GetNewsAsync(string rssUrl, string categoryName = "")
            => NewsDL.GetNewsAsync(rssUrl, categoryName);
    }
}