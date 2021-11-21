using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Identity.Domain.AggregatesModel.IdentityAggregate;

namespace Identity.Domain.Security.Tokens
{
    public interface ITokenHandler
    {
        AccessToken CreateAccessToken(AppIdentityUser user);
    }
}
