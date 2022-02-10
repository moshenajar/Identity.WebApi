using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.WebApi.Controllers.Resources
{
    public class RevokeTokenResource
    {
        [Required]
        public string Email { get; set; }
    }
}
