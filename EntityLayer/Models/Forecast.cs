namespace EntityLayer.Models
{
    
    /// Günlük hava durumu tahmini
   
    public class Forecast
    {
        public string Date { get; set; } = "";
        public string DayName { get; set; } = "";
        public string MaxTemp { get; set; } = "";
        public string MinTemp { get; set; } = "";
        public string Condition { get; set; } = "";
        public string Icon { get; set; } = "";
    }
}