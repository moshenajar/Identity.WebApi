using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace Identity.WebApi.Security.Tokens
{
    public class SigningConfigurations
    {
        public SecurityKey SecurityKey { get; }
        public SigningCredentials SigningCredentials { get; }

        public SigningConfigurations(string key)
        {
            var keyBytes = Encoding.UTF8.GetBytes(key);

            SecurityKey = new SymmetricSecurityKey(keyBytes);
            SigningCredentials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256Signature);
        }
    }
}
