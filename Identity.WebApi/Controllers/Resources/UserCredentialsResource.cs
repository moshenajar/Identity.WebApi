using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.WebApi.Controllers.Resources
{
    public class UserCredentialsResource
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string NewPassword { get; set; }
        public DateTime Birthday { get; set; }
        public int Gender { get; set; }
        public string Email { get; set; }
    }
}
