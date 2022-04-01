using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.Security.Otp
{
    public class OtpResponse
    {
        public string description { get; set; }
        public int code { get; set; }
        public Entities entities { get; set; }
        public Password password { get; set; }
        public string id { get; set; }
        public OtpResponse() { }
        public OtpResponse(string description, int code, Entities entities, Password password, string id)
        {
            this.description = description;
            this.code = code;
            this.entities = entities;
            this.password = password;
            this.id = id;
        }
    }
}
