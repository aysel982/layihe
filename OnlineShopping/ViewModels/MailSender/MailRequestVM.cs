namespace OnlineShopping.ViewModels.MailSender
{
    public class MailRequestVM
    {
        public string emailTo { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public List<IFormFile> Attachments { get; set; }
    }
}
