namespace OnlineShopping.ViewModels
{
    public class MailRequestVM
    {
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public List<IFormFile> Attachment { get; set; }
    }
}
