using NuGet.Protocol.Core.Types;

namespace OnlineShopping.ViewModels
{
    public class BasketItemVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public decimal Price { get; set; }
        public decimal Subtotal { get; set; }
        public int Count { get; set; }
    }
}
