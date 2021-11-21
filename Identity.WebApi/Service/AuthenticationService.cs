using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Identity.Domain.AggregatesModel.IdentityAggregate;
using Identity.Domain.Helpers;
using Identity.Domain.Repositories;
using Identity.Domain.Security.Hashing;
using Identity.Domain.Security.Tokens;
using Identity.Domain.Service;
using Identity.Domain.Service.Communication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Web;

namespace Identity.WebApi.Service
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _userRepository;
        private readonly AppUserManager _userManager;
        //private readonly IUserService _userService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenHandler _tokenHandler;
        private readonly IEmailService _emailService;
        private readonly ApiUrlSettings _apiUrlSettings;

        public AuthenticationService(
            IUserRepository userRepository,
            AppUserManager userManager,
            IUserService userService,
            IPasswordHasher passwordHasher,
            ITokenHandler tokenHandler,
            IEmailService emailService,
            IOptions<ApiUrlSettings> apiUrlSettings)
        {
            _userManager = userManager;
            _tokenHandler = tokenHandler;
            _passwordHasher = passwordHasher;
            //_userService = userService;
            _userRepository = userRepository;
            _emailService = emailService;
            _apiUrlSettings = apiUrlSettings.Value;
        }

        public async Task<TokenResponse> CreateAccessTokenAsync(string email, string password)
        {
            const string succeededButEmailFail = "Login can not be performed until email verification, but Email verification failed, Please try again later.";
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new TokenResponse(false, "Invalid credentials.", 1, null);
            }

            if (user != null)
            {
                if (!_userManager.IsEmailConfirmedAsync
                     (user).Result)
                {
                    string confirmationToken = _userManager.
                        GenerateEmailConfirmationTokenAsync(user).Result;


                    confirmationToken = user.Id.ToString() + "confirmation" + HttpUtility.UrlEncode(confirmationToken);

                    string verifyUrl =
                        "<a href='" + _apiUrlSettings.ApiConfirmEmail + confirmationToken + "'>אישור</a>";

                    try
                    {
                        sendVerificationEmail(user, verifyUrl);
                    }
                    catch (Exception e)
                    {
                        return new TokenResponse(true, succeededButEmailFail, 1, null);
                    }


                    return new TokenResponse(false, "לא בוצע אימות דוא''ל, נשלח אליך דוא''ל אימות נוסף", 401, null);
                }
            }

            var result = await _userManager.CheckPasswordAsync(user, password);

            if (!result)
            {
                return new TokenResponse(false, "Invalid credentials.", 1, null);
            }

            var accessToken = _tokenHandler.CreateAccessToken(user);

            await _userManager.SetAuthenticationTokenAsync(user, "MyIsrael", "Token", accessToken.Token);
            await _userManager.SetAuthenticationRefreshTokenAsync(user, accessToken.RefreshToken.Token, accessToken.RefreshToken.Expiration);

            return new TokenResponse(true, null, 0, accessToken);
        }

        public async Task<TokenResponse> RefreshTokenAsync(string refreshToken, string userEmail)
        {
            var token = await _userRepository.TakeRefreshToken(refreshToken);

            if (token == null)
            {
                return new TokenResponse(false, "Invalid refresh token.", 0, null);
            }

            if (token.IsExpired())
            {
                return new TokenResponse(false, "Expired refresh token.", 0, null);
            }

            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                return new TokenResponse(false, "Invalid refresh token.", 0, null);
            }


            var accessToken = _tokenHandler.CreateAccessToken(user);

            await _userManager.SetAuthenticationTokenAsync(user, "MyIsrael", "Token", accessToken.Token);
            await _userManager.SetAuthenticationRefreshTokenAsync(user, accessToken.RefreshToken.Token, accessToken.RefreshToken.Expiration);

            return new TokenResponse(true, null, 0, accessToken);
        }

        public void RevokeRefreshToken(string refreshToken)
        {
            _userRepository.RevokeRefreshToken(refreshToken);
        }

        public async Task<IdentityResult> RevokeToken(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var result = await _userManager.RemoveAuthenticationTokenAsync(user, "MyIsrael", "Token");

            return result;
        }

        private void sendVerificationEmail(AppIdentityUser account, string verifyUrl)
        {
            string message;
            //if (!string.IsNullOrEmpty(verifyUrl))
            //{
            //var verifyUrl = $"{origin}/account/verify-email?token={account.VerificationToken}";
            message = $@"<p>Please click the below link to verify your email address:</p>
                             <p><a href=""{verifyUrl}"">{verifyUrl}</a></p>";
            //}
            //else
            //{
            //    message = $@"<p>Please use the below token to verify your email address with the <code>/accounts/verify-email</code> api route:</p>
            //                 <p><code>{account.VerificationToken}</code></p>";
            //}

            _emailService.Send(
                to: account.Email,
                subject: "Sign-up Verification API - Verify Email",
                html: $@"<h4>Verify Email</h4>
                         <p>Thanks for registering!</p>
                         {message}"
                );
        }

    }
}
