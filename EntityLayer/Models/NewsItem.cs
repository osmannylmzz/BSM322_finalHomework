namespace EntityLayer.Models
{
    /// Tek bir haber öğesi
  
    public class NewsItem
    {
        public string Title { get; set; } = "";
        public string Link { get; set; } = "";
        public string Description { get; set; } = "";
        public string PubDate { get; set; } = "";
        public string Thumbnail { get; set; } = "";
        public string Category { get; set; } = "";
    }
}