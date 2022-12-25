using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace GadgetMarket.Web.Model
{
    public class Auth
    {
        private const string KEY = "mysupersecret_secretkey!123";

        public const string ISSUER = "MyAuthServer";
        public const string AUDIENCE = "MyAuthClient";
        public const int LIFETIME = 100;

        public static SymmetricSecurityKey GetSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}
