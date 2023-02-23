using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Thss0.DAL.Config
{
    internal class AuthCredentials
    {
        public const string ISSUER = "Server";
        public const string AUDIENCE = "Client";
        public const int LIFETIME = 10;
        private const string KEY = "0000000000";
        public static SymmetricSecurityKey GetKey()
            => new SymmetricSecurityKey(Encoding.Default.GetBytes(KEY));
    }
}