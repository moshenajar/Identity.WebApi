using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.WebApi.Controllers.Resources
{
    public class OtpResource
    {
        public RequestIdentifiers requestIdentifiers { get; set; }
        public int systemId { get; set; }
        public Recipient recipient { get; set; }
        public SendPolicies sendPolicies { get; set; }
    }

    public class RequestIdentifiers
    {
        public string sessionID { get; set; }
        public string module { get; set; }
    }

    public class Recipient
    {
        public string mobile { get; set; }
        public string email { get; set; }
    }

    public class SendPolicies
    {
        public string distributionType { get; set; }
    }
}
