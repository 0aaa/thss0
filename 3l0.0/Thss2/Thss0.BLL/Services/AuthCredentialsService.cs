using Microsoft.IdentityModel.Tokens;
using Thss0.DAL.Config;

namespace Thss0.BLL.Services
{
    public class AuthCredentialsService
    {
        public static string GetIssuer() => AuthCredentials.ISSUER;
        public static string GetAudience() => AuthCredentials.AUDIENCE;
        public static int GetLifetime() => AuthCredentials.LIFETIME;
        public static SymmetricSecurityKey GetKey() => AuthCredentials.GetKey();
    }
}