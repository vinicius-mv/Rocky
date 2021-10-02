using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;

// REF: https://www.c-sharpcorner.com/article/send-email-using-asp-net-core-5-web-api/
namespace Rocky.Utility.EmailPackage
{
    public class EmailService : IEmailService, IEmailSender
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> mailSettings)
        {
            _emailSettings = mailSettings.Value;
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
            email.Sender = MailboxAddress.Parse(_emailSettings.Mail);
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
            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            smtp.Connect(_emailSettings.Host, _emailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_emailSettings.Mail, _emailSettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            if(email == WebConstants.EmailAdmin)
                email = _emailSettings.Mail;

            var mailRequest = new MailRequest { ToEmail = email, Subject = subject, Body = htmlMessage };
            await SendEmailAsync(mailRequest);
        }
    }
}
