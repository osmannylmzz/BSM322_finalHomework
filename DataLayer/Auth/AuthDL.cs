using System;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Auth.Providers;
using DataLayer.Config;

namespace DataLayer.Auth
{
    public static class AuthDL
    {
        private static FirebaseAuthConfig BuildConfig() => new FirebaseAuthConfig
        {
            ApiKey = FirebaseSettings.ApiKey,
            AuthDomain = FirebaseSettings.AuthDomain,
            Providers = new FirebaseAuthProvider[] { new EmailProvider() }
        };

        private static string? ExtractUid(UserCredential? credential)
        {
            try
            {
                var user = credential?.User;
                if (user is null) return null;

                // FirebaseAuthentication.net v4.x sadece Uid kullanıyor
                if (!string.IsNullOrWhiteSpace(user.Uid))
                    return user.Uid;

                // Info üzerinden de denenebilir (bazı edge case'ler için)
                if (user.Info != null && !string.IsNullOrWhiteSpace(user.Info.Uid))
                    return user.Info.Uid;

                return null;
            }
            catch
            {
                return null;
            }
        }

        public static async Task<(bool ok, string error, string? uid)> RegisterAsync(string username, string email, string password)
        {
            try
            {
                var client = new FirebaseAuthClient(BuildConfig());
                var res = await client.CreateUserWithEmailAndPasswordAsync(email, password, username);

                var uid = ExtractUid(res);
                var ok = res?.User != null && !string.IsNullOrWhiteSpace(uid);

                return (ok, ok ? "" : "Kayıt başarılı ancak uid alınamadı.", uid);
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }
        }

        public static async Task<(bool ok, string error, string? uid)> LoginAsync(string email, string password)
        {
            try
            {
                var client = new FirebaseAuthClient(BuildConfig());
                var res = await client.SignInWithEmailAndPasswordAsync(email, password);

                var uid = ExtractUid(res);
                var ok = res?.User != null && !string.IsNullOrWhiteSpace(uid);

                return (ok, ok ? "" : "Giriş başarılı ancak uid alınamadı.", uid);
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }
        }
    }
}
