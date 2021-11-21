using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Identity.Domain.AggregatesModel.IdentityAggregate;
using Identity.Domain.Repositories;
using Identity.Domain.Security.Tokens;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Identity.WebApi.Service
{
    public class AppUserManager : UserManager<AppIdentityUser>
    {
        private readonly IUserRepository _userRepository;
        public AppUserManager(IUserRepository userRepository, IUserStore<AppIdentityUser> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<AppIdentityUser> passwordHasher, IEnumerable<IUserValidator<AppIdentityUser>> userValidators, IEnumerable<IPasswordValidator<AppIdentityUser>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<AppIdentityUser>> logger) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            _userRepository = userRepository;
        }

        public async Task SetAuthenticationRefreshTokenAsync(AppIdentityUser user, string token, long expiration)
        {
            //Store.
            await _userRepository.AddRefreshUserToken(user, new RefreshToken(token, expiration));
        }

    }
}
