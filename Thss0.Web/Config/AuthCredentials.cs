using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Thss0.Web.Config
{
    public class AuthCredentials
    {
        public const string SUBSTANCES_API_KEY = "v4hoSfforrN7hbzLrJMDFdNW0UA0W01XcqY40bBR";
        public const string ISSUER = "Server";
        public const string AUDIENCE = "Client";
        public const int LIFETIME = 15;
        private const string SIGNING_KEY = "KeyKeyKeyKeyKeyKeyKeyKeyKeyKey";
        public static SymmetricSecurityKey GetSigningKey()
            => new(Encoding.Default.GetBytes(SIGNING_KEY));
    }
}