namespace EntityLayer.Models
{

    /// Şehir hava durumu bilgisi
    
    public class CityWeather
    {
        public string CityName { get; set; } = "";
        public string Country { get; set; } = "";
        public string CurrentTemp { get; set; } = "";
        public string CurrentCondition { get; set; } = "";
        public string Humidity { get; set; } = "";
        public string WindSpeed { get; set; } = "";
        public string FeelsLike { get; set; } = "";
        public List<Forecast> Forecasts { get; set; } = new();
    }
}