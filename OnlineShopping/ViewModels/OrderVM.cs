namespace OnlineShopping.ViewModels
{
    public class OrderVM
    {
        public string Adress { get; set; }
        public List<BasketInOrderVM>? BasketInOrderVMs { get; set; }
    }
}
