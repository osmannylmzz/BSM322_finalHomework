using static System.Net.WebRequestMethods;

namespace DataLayer.Config
{
    public static class FirebaseSettings
    {
        public static string ProjectId = "authentication-4a3df";
        public static string ApiKey = "AIzaSyAZIYo1MWLFoMugLpG3eKJWqL1uxeu3Uwk";
        public static string AuthDomain = "authentication-4a3df.firebaseapp.com";

        // ÖNEMLİ: FirebaseDatabase.net için Console URL değil, DB root URL gerekir.
        // Format: https://{db-name}.firebaseio.com/
        // Sonunda / olmalı.
        public static string RealtimeDbUrl = "https://authentication-4a3df-default-rtdb.europe-west1.firebasedatabase.app/";
    }
}
