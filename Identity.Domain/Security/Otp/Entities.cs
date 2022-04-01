using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.Security.Otp
{
    public class Entities
    {
        public Sent sent { get; set; }
        public long expiredTime { get; set; }
        public int attemptsNumber { get; set; }
    }

}
