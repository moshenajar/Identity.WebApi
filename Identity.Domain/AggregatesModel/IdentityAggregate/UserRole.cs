using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.AggregatesModel.IdentityAggregate
{
    public enum ApplicationRole
    {
        Common = 1,
        Administrator = 2
    }

    public static class RoleExtensions
    {
        public static string GetRoleName(this ApplicationRole applicationRole)
        {
            return applicationRole.ToString();
        }
    }
}
