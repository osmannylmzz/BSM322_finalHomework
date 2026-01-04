using static System.Net.WebRequestMethods;

namespace DataLayer.Config
{
    public static class FirebaseSettings
    {
        public static string ProjectId = "";
        public static string ApiKey = "";
        public static string AuthDomain = "";

        // ÖNEMLİ: FirebaseDatabase.net için Console URL değil, DB root URL gerekir.
        // Format: https://{db-name}.firebaseio.com/
        // Sonunda / olmalı.
        public static string RealtimeDbUrl = "/";
    }
}
