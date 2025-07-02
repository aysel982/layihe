using OnlineShopping.ViewModels;

namespace OnlineShopping.Services.Interfaces
{
    public interface IMailService
    {
        Task SendMailAsync(MailRequestVM mailRequestVM);
    }
}
