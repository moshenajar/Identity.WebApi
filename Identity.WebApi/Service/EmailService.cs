using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Identity.Domain.Helpers;
using Identity.Domain.Service;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace Identity.WebApi.Service
{
    public class EmailService : IEmailService
    {
        private readonly EmailAppSettings _appSettings;

        public EmailService(IOptions<EmailAppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public void Send(string to, string subject, string html, string from = null)
        {
            try
            {
                // create message
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(from ?? _appSettings.EmailFrom));
                email.To.Add(MailboxAddress.Parse(to));
                email.Subject = subject;
                email.Body = new TextPart(TextFormat.Html) { Text = html };

                // send email
                using var smtp = new SmtpClient();
                smtp.Connect(_appSettings.SmtpHost, _appSettings.SmtpPort, SecureSocketOptions.StartTls);
                smtp.Authenticate(_appSettings.SmtpUser, _appSettings.SmtpPass);
                smtp.Send(email);
                smtp.Disconnect(true);
            }
            catch (MailKit.Security.AuthenticationException e)
            {
                throw new MailKit.Security.AuthenticationException(e.Message, e.InnerException);
            }

        }
    }
}
