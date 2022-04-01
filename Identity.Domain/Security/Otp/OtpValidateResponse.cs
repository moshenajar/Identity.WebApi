using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.Security.Otp
{
    public class OtpValidateResponse
    {
        public string description { get; set; }
        public int code { get; set; }
        public Entities entities { get; set; }
        public OtpValidateResponse() { }
        public OtpValidateResponse(string description, int code, long expiredTime, int attemptsNumber)
        {
            this.description = description;
            this.code = code;
            this.entities.sent = null;
            this.entities.expiredTime = expiredTime;
            this.entities.attemptsNumber = attemptsNumber;
        }
    }
}
