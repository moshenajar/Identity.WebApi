using Identity.Domain.AggregatesModel.IdentityAggregate;
using Identity.Domain.Repositories;
using Identity.Domain.Security.Tokens;
using Identity.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.WebApi.Service
{
    public class UserRepository : IUserRepository
    {
        private readonly UserDbContext _context;

        public UserRepository(UserDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(AppIdentityUser user, ApplicationRole[] userRoles)
        {
            var roleNames = userRoles.Select(r => r.ToString()).ToList();
            var roles = await _context.Roles.Where(r => roleNames.Contains(r.Name)).ToListAsync();

            foreach (var role in roles)
            {
                //user.UserRoles.Add(new UserRole { RoleId = role.Id });
            }

            _context.Users.Add(user);

        }

        public async Task<AppIdentityUser> FindByEmailAsync(string email)
        {
            return await _context.Users//.Include(u => u.UserRoles)
                                       //.ThenInclude(ur => ur.Role)
                                       .SingleOrDefaultAsync(u => u.Email == email);
        }

        public async Task AddRefreshUserToken(AppIdentityUser user, RefreshToken refreshToken)
        {
            var _appIdentityUserRefreshToken = await _context.AppIdentityUserRefreshTokens
                    .SingleOrDefaultAsync(r => r.UserId == user.Id);

            if (_appIdentityUserRefreshToken != null)
            {
                _context.AppIdentityUserRefreshTokens.Remove(_appIdentityUserRefreshToken);
                await _context.SaveChangesAsync();
            }
            //RemoveRefreshUserToken(_appIdentityUserRefreshToken);



            await _context.AppIdentityUserRefreshTokens.AddAsync(
                new AppIdentityUserRefreshToken
                {
                    UserId = user.Id,
                    LoginProvider = "MyIsrael",
                    Name = "refreshToken",
                    Value = refreshToken.Token,
                    Expiration = refreshToken.Expiration
                });
            await _context.SaveChangesAsync();
        }

        public async Task<AppIdentityUserRefreshToken> FindByRefreshTokenAsync(string token)
        {
            return await _context.AppIdentityUserRefreshTokens
                .SingleOrDefaultAsync(r => r.Value == token);
        }

        public async Task<AppIdentityUserToken> FindByTokenAsync(string token)
        {
            return await _context.AppIdentityUserTokens
                .SingleOrDefaultAsync(r => r.Value == token);
        }

        public void RemoveRefreshUserToken(AppIdentityUserRefreshToken refreshToken)
        {
            _context.AppIdentityUserRefreshTokens.Remove(refreshToken);
            _context.SaveChangesAsync();
        }

        public async Task<RefreshToken> TakeRefreshToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return null;

            var refreshToken = await FindByRefreshTokenAsync(token);

            if (refreshToken == null)
                return null;

            _context.AppIdentityUserRefreshTokens.Remove(refreshToken);
            await _context.SaveChangesAsync();

            return new RefreshToken(refreshToken.Value, refreshToken.Expiration);
        }

        public async Task RevokeRefreshToken(string token)
        {
            await TakeRefreshToken(token);
        }

        public async Task<AppIdentityUserToken> TakeToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return null;

            var Token = await FindByTokenAsync(token);

            if (Token == null)
                return null;

            _context.AppIdentityUserTokens.Remove(Token);
            await _context.SaveChangesAsync();

            return Token;
        }

        public async Task RevokeToken(string token)
        {
            await TakeToken(token);
        }
    }
}