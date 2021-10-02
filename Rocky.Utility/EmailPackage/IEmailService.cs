using System.Threading.Tasks;

namespace Rocky.Utility.EmailPackage
{
    public interface IEmailService
    {
        Task SendEmailAsync(MailRequest mailRequest);
    }
}
