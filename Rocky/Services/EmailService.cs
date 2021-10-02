using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Rocky.Models;
using System.IO;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Rocky.Settings;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;

// REF: https://www.c-sharpcorner.com/article/send-email-using-asp-net-core-5-web-api/
namespace Rocky.Services
{
    public class EmailService : IEmailService, IEmailSender
    {
        private readonly EmailSettings _mailSettings;

        public EmailService(IOptions<EmailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        // another way to read EmailSettings from app.settings.json
        //private readonly IConfiguration _config;
        //private readonly EmailSettings _mailSettings;
        //public EmailService(IConfiguration config)
        //{
        //    _config = config;
        //    _mailSettings = _config.GetSection("EmailSettings").Get<EmailSettings>();
        //}

        public async Task SendEmailAsync(MailRequest mailRequest)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
            email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
            email.Subject = mailRequest.Subject;
            var builder = new BodyBuilder();
            if (mailRequest.Attachments != null)
            {
                byte[] fileBytes;
                foreach (var file in mailRequest.Attachments)
                {
                    if (file.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            file.CopyTo(ms);
                            fileBytes = ms.ToArray();
                        }
                        builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
                    }
                }
            }
            builder.HtmlBody = mailRequest.Body;
            email.Body = builder.ToMessageBody();
            using var smtp = new SmtpClient();
            smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var mailRequest = new MailRequest { ToEmail = email, Subject = subject, Body = htmlMessage };
            await SendEmailAsync(mailRequest);
        }
    }
}
