using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Identity.Domain.Security.Tokens;

namespace Identity.Domain.Service.Communication
{
    public class TokenResponse : BaseResponse
    {
        public AccessToken response { get; set; }

        public TokenResponse(bool success, string message, int resultCode, AccessToken token) : base(success, message, resultCode)
        {
            response = token;
        }
    }
}
