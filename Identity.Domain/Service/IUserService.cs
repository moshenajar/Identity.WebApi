using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Identity.Domain.AggregatesModel.IdentityAggregate;
using Identity.Domain.Service.Communication;

namespace Identity.Domain.Service
{
    public interface IUserService
    {
        Task<TokenResponse> CreateUserAsync(AppIdentityUser user, string newPassword, params ApplicationRole[] userRoles);
        Task<AppIdentityUser> FindByEmailAsync(string email);
        Task<Response> ConfirmEmail(string token);
        Task<CreateUserResponse> ForgotPassword(string email);
        Task<CreateUserResponse> ResetPassword(string Email, string Token, string ConfirmPassword);
    }
}
