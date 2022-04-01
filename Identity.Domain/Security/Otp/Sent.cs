using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.Security.Otp
{
    public class Sent
    {
        public string distributionType { get; set; }
        public int status { get; set; }
        public string message { get; set; }
        public string address { get; set; }
    }
}
