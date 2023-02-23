using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Thss0.DAL.Config
{
    public class AuthCredentials
    {
        public const string ISSUER = "Server";
        public const string AUDIENCE = "Client";
        public const int LIFETIME = 15;
        private const string KEY = "KeyKeyKeyKeyKeyKeyKeyKeyKeyKey";
        public static SymmetricSecurityKey GetKey()
            => new SymmetricSecurityKey(Encoding.Default.GetBytes(KEY));
    }
}
