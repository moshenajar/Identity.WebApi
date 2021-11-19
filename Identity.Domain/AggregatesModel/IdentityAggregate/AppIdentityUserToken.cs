using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.AggregatesModel.IdentityAggregate
{
    public class AppIdentityUserToken : IdentityUserToken<Guid>
    {
    }
}
