namespace EntityLayer.Models
{
    
    /// Döviz/Altýn kur bilgisi modeli
  
    public class Currency
    {
        public string Code { get; set; } = "";       // Örn: "USD", "EUR", "GA"
        public string Name { get; set; } = "";       // Örn: "Amerikan Dolarý", "Gram Altýn"
        public string Buying { get; set; } = "";     // Alýþ fiyatý
        public string Selling { get; set; } = "";    // Satýþ fiyatý
        public string Change { get; set; } = "";     // Deðiþim yüzdesi
    }
}