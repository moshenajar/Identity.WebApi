using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Identity.Domain.Service.Communication;
using Microsoft.AspNetCore.Identity;

namespace Identity.Domain.Service
{
    public interface IAuthenticationService
    {
        Task<TokenResponse> CreateAccessTokenAsync(string email, string password);
        Task<TokenResponse> RefreshTokenAsync(string refreshToken, string userEmail);
        void RevokeRefreshToken(string refreshToken);
        Task<IdentityResult> RevokeToken(string email);
    }
}
