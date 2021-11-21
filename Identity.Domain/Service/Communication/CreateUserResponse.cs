using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.Service.Communication
{
    public class CreateUserResponse : BaseResponse
    {
        //public AppIdentityUser User { get; private set; }

        public CreateUserResponse(bool success, string message, int resultCode/*, AppIdentityUser user*/) : base(success, message, resultCode)
        {
            /*User = user;*/
        }
    }
}
