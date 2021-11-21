using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Identity.Domain.AggregatesModel.IdentityAggregate;
using Identity.Domain.Security.Tokens;

namespace Identity.Domain.Repositories
{
    public interface IUserRepository
    {
        Task AddAsync(AppIdentityUser user, ApplicationRole[] userRoles);
        Task<AppIdentityUser> FindByEmailAsync(string email);
        Task AddRefreshUserToken(AppIdentityUser user, RefreshToken refreshToken);
        Task<AppIdentityUserRefreshToken> FindByRefreshTokenAsync(string token);
        void RemoveRefreshUserToken(AppIdentityUserRefreshToken refreshToken);
        Task<RefreshToken> TakeRefreshToken(string token);
        Task RevokeRefreshToken(string token);
        Task RevokeToken(string token);
    }
}
