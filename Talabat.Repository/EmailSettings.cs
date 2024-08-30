using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Talabat.APIs.Helper;
using Talabat.Core.Entities;
using Talabat.Core.Services;

namespace Talabat.Repository
{
    public class EmailSettings : IEmailSettings
    {
        private MailSettings _options;

        public EmailSettings(IOptions<MailSettings> options)
        {
            _options = options.Value;
        }


        public void SendEmail(Email email)
        { 
            var mail = new MimeMessage
            {
                Sender = MailboxAddress.Parse(_options.Email),
                Subject =  email.Subject,
            };

            mail.To.Add(MailboxAddress.Parse(email.To));

            // Build The Email Body 
            var builder = new BodyBuilder();

            builder.TextBody = email.Body;

            mail.Body = builder.ToMessageBody();

            // To put the DisplayName not sender's email
            mail.From.Add(new MailboxAddress(_options.DisplayName, _options.Email));

            // To Connect to Mail Provider -> smtp
            using var smtp = new SmtpClient();

            smtp.Connect(_options.Host, _options.Port, SecureSocketOptions.StartTls);

            smtp.Authenticate(_options.Email, _options.Password);

            smtp.Send(mail);

            smtp.Disconnect(true);

        }
    }
}
