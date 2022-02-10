using Identity.Domain.AggregatesModel.IdentityAggregate;
using Identity.Domain.Security.Hashing;
using Identity.Domain.Service;
using Identity.Domain.Service.Communication;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace Identity.WebApi.Service
{
    public class UserService: IUserService
    {
        private readonly AppUserManager _userManager;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IEmailService _emailService;


        public UserService(AppUserManager userManager,
            IPasswordHasher passwordHasher,
            IEmailService emailService)
        {
            _userManager = userManager;
            _passwordHasher = passwordHasher;
            _emailService = emailService;
        }

        public async Task<CreateUserResponse> CreateUserAsync(AppIdentityUser user, string newPassword, params ApplicationRole[] userRoles)
        {
            string errorCode = string.Empty;
            const string succeeded = "Registration successful, please check your email verification instructions";
            IdentityResult result = null;

            var existingUser = await _userManager.FindByEmailAsync(user.Email);
            if (existingUser != null)
            {
                return new CreateUserResponse(false, "Email already in use.", 1);
            }

            result = await _userManager.CreateAsync(user, newPassword);

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (var error in result.Errors)
                    {
                        errorCode = error.Code;
                    }
                }


                return new CreateUserResponse(false, errorCode, 1);
            }


            _userManager.AddToRolesAsync(user, userRoles.Select(r => r.GetRoleName())).Wait();// _applicationRoles.Select(r => r.GetRoleName()));

            string confirmationToken = _userManager.
                 GenerateEmailConfirmationTokenAsync(user).Result;

            confirmationToken = HttpUtility.UrlEncode(confirmationToken);

            /********************************************************************************/
            //send confirmation email

            string verifyUrl = "http://localhost:4200/confirmemail/" + user.Id + "/" + confirmationToken;

            // send email
            //sendVerificationEmail(user, verifyUrl);

            return new CreateUserResponse(true, succeeded, 0);
        }

        public async Task<AppIdentityUser> FindByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<Response> ConfirmEmail(string token)
        {
            string message = string.Empty;
            int resultCode = -1;

            AppIdentityUser user = await _userManager.FindByIdAsync("ToDo");
            IdentityResult result = await _userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
            {
                message = "Email confirmed successfully!";
                resultCode = 0;
            }
            else
            {
                message = "Error while confirming your email!";
            }

            return new Response(true, message, resultCode);
        }

        public async Task<CreateUserResponse> ForgotPassword(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return new CreateUserResponse(false, "Email not found.", 1);

            string GeneratePasswordResetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            GeneratePasswordResetToken = HttpUtility.UrlEncode(GeneratePasswordResetToken);


            /********************************************************************************/
            //send confirmation email

            string confirmationLink =
                "<a href='http://localhost:4200/ForgotPasswordConfirmation/" + user.Id + "/" + GeneratePasswordResetToken + "'>אישור</a>";

            MailMessage msg = null;
            // Gmail Address from where you send the mail 
            var fromAddress = "myisraeltest@gmail.com";
            // any address where the email will be sending 
            var toAddress = user.Email;
            //Password of your gmail address 
            const string fromPassword = "Unix11!1q2w3e4r";
            // Passing the values and make a email formate to display 
            string subject = "Confirm your email";// YourSubject.Text.ToString();
            string body = confirmationLink;

            // smtp settings 
            var smtp = new System.Net.Mail.SmtpClient();
            {
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(fromAddress, fromPassword);
                smtp.Timeout = 20000;
            }

            msg = new MailMessage(fromAddress,
                toAddress, subject,
                body);

            msg.IsBodyHtml = true;

            smtp.Send(msg);


            /********************************************************************************/

            return new CreateUserResponse(true, "succeeded", 0);
        }

        public async Task<CreateUserResponse> ResetPassword(string Email, string Token, string Password)
        {
            var user = await _userManager.FindByEmailAsync(Email);

            if (user == null)
                return new CreateUserResponse(false, "Email not found.", 1);

            var resetPassResult = await _userManager.ResetPasswordAsync(user, Token, Password);
            if (!resetPassResult.Succeeded)
            {
                foreach (var error in resetPassResult.Errors)
                {
                    //        ModelState.TryAddModelError(error.Code, error.Description);
                }
                return new CreateUserResponse(false, "Error.", 1);
            }
            return new CreateUserResponse(true, "succeeded", 0);
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
