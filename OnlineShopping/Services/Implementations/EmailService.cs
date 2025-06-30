using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using OnlineShopping.Services.Interfaces;
using System.Net;
using System.Net.Mail;

namespace OnlineShopping.Services.Implementations
{
    public class EmailService:IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendMailAsync(string emailTo,string subject,string body,bool isHtml=false)
        {
            SmtpClient smtp = new SmtpClient(_configuration["Email:Host"], Convert.ToInt32(_configuration["Email:Post"]));
            smtp.EnableSsl = true;
            smtp.Credentials = new NetworkCredential(_configuration["Email:LoginEmail"], _configuration["Email:Password"]);
            MailAddress from = new MailAddress(_configuration["Email:LoginEmail"]);
            MailAddress to = new MailAddress(emailTo);

            MailMessage message = new MailMessage(from, to);
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = isHtml;

            await smtp.SendMailAsync(message);
        }
    }
}
