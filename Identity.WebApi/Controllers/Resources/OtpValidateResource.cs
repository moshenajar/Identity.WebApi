using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.WebApi.Controllers.Resources
{
    public class OtpValidateResource
    {
        public string id { get; set; }
        public string otp { get; set; }
    }
}
