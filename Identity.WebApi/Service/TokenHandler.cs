using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Identity.Domain.AggregatesModel.IdentityAggregate;
using Identity.Domain.Security.Hashing;
using Identity.Domain.Security.Tokens;
using Identity.WebApi.Security.Tokens;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Identity.WebApi.Service
{
    public class TokenHandler : ITokenHandler
    {
        //private readonly ISet<RefreshToken> _refreshTokens = new HashSet<RefreshToken>();

        //private readonly IApplicationDbContext _dbContext;
        //private readonly IUserRepository _userRepository;
        //private readonly AppUserManager _userManager;
        private readonly TokenOptions _tokenOptions;//class
        private readonly SigningConfigurations _signingConfigurations;//AddSingleton
        private readonly IPasswordHasher _passwordHaser;//AddSingleton
        //private readonly IUserRepository _userRepository;

        public TokenHandler(
            //AppUserManager userManager,
            IOptions<TokenOptions> tokenOptionsSnapshot,
            SigningConfigurations signingConfigurations,
            IPasswordHasher passwordHaser//,
                                         //IUserRepository userRepository
            )
        {
            //_userManager = userManager;
            _passwordHaser = passwordHaser;
            _tokenOptions = tokenOptionsSnapshot.Value;
            _signingConfigurations = signingConfigurations;
            //_userRepository = userRepository;
        }

        public AccessToken CreateAccessToken(AppIdentityUser user)
        {
            var refreshToken = BuildRefreshToken();
            //var accessToken = BuildAccessToken(user, refreshToken);
            var accessToken = this.CreateToken(user, refreshToken);
            return accessToken;
        }


        private RefreshToken BuildRefreshToken()
        {
            var refreshToken = new RefreshToken
            (
                token: _passwordHaser.HashPassword(Guid.NewGuid().ToString()),
                expiration: DateTime.UtcNow.AddSeconds(_tokenOptions.RefreshTokenExpiration).Ticks
            );

            return refreshToken;
        }

        private AccessToken BuildAccessToken(AppIdentityUser user, RefreshToken refreshToken)
        {
            var accessTokenExpiration = DateTime.UtcNow.AddSeconds(_tokenOptions.AccessTokenExpiration);

            var securityToken = new JwtSecurityToken
            (
                issuer: _tokenOptions.Issuer,
                audience: _tokenOptions.Audience,
                claims: GetClaims(user),
                expires: accessTokenExpiration,
                //notBefore: DateTime.UtcNow,
                signingCredentials: _signingConfigurations.SigningCredentials
            );

            var handler = new JwtSecurityTokenHandler();
            var accessToken = handler.WriteToken(securityToken);

            return new AccessToken(accessToken, accessTokenExpiration.Ticks, refreshToken);
        }

        private AccessToken CreateToken(AppIdentityUser user, RefreshToken refreshToken)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                //new Claim(ClaimTypes.Name, user.Username)
            };

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes("very_long_but_insecure_token_here_be_sure_to_use_env_var"));

            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = "JWPAPI",
                Audience = "SampleAudience",
                Subject = new ClaimsIdentity(GetClaims(user)),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            string accessToken = tokenHandler.WriteToken(token);

            return new AccessToken(accessToken, tokenDescriptor.Expires.Value.Ticks, refreshToken);

        }

        //private IEnumerable<Claim> GetClaims(User user)
        //{
        //    var claims = new List<Claim>
        //    {
        //        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        //        new Claim(JwtRegisteredClaimNames.Sub, user.Email)
        //    };

        //    foreach (var userRole in user.UserRoles)
        //    {
        //        claims.Add(new Claim(ClaimTypes.Role, userRole.Role.Name));
        //    }

        //    return claims;
        //}

        private IEnumerable<Claim> GetClaims(AppIdentityUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName),
                new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName),
                new Claim(JwtRegisteredClaimNames.Gender, user.Gender.ToString()),
                new Claim("EmailConfirmed", user.EmailConfirmed == true ? "1" : "0"),
                new Claim("PhoneNumberConfirmed", user.PhoneNumberConfirmed == true ? "1" : "0")
            };


            //    foreach (var userRole in user.UserRoles)
            //    {
            //        claims.Add(new Claim(ClaimTypes.Role, userRole.Role.Name));
            //    }

            return claims;
        }

    }
}
