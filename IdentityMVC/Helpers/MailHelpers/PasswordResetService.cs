using IdentityMVC.Models.Entities;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace IdentityMVC.Helpers.MailHelpers
{
    public class PasswordResetService
    {
        readonly SmtpSettings _smtpSettings;
        public PasswordResetService(IOptions<SmtpSettings> smtpSettings)
        {
            _smtpSettings = smtpSettings.Value;
        }

        public void PasswordResetSendEmail(string passwordRefreshLink,string email)
        {
            MailMessage mailMessage = new MailMessage
            {
                From = new MailAddress(_smtpSettings.SenderEmail),
                To = { new MailAddress(email) },
                Subject = "www.identitycourse.com - Reset Password",
                IsBodyHtml = true,
                Body = @$"<a href=""{passwordRefreshLink}""> Reset Password Link </a>"
            };

            SmtpClient smtpClient = new SmtpClient
            {
                Host = _smtpSettings.Host,
                Port = _smtpSettings.Port,
                EnableSsl = true,
                UseDefaultCredentials = false,//kendi hesap bilgileri vereceğiz..
                Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password),
                DeliveryMethod = SmtpDeliveryMethod.Network,
            };
            smtpClient.Send(mailMessage);
        }


      
    }
}
