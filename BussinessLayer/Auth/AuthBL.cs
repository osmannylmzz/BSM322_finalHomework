using System.Threading.Tasks;
using DataLayer.Auth;

namespace BusinessLayer.Auth
{
    public static class AuthBL
    {
        public static Task<(bool ok, string error, string? uid)> RegisterAsync(string username, string email, string password)
            => AuthDL.RegisterAsync(username, email, password);

        public static Task<(bool ok, string error, string? uid)> LoginAsync(string email, string password)
            => AuthDL.LoginAsync(email, password);
    }
}
    