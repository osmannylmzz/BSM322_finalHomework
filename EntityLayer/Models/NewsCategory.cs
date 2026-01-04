namespace EntityLayer.Models
{
 
    /// Haber kategorisi
   
    public class NewsCategory
    {
        public string Name { get; set; } = "";       // Görünen ad: "Manþet", "Son Dakika"
        public string RssUrl { get; set; } = "";     // RSS feed URL'si
    }
}