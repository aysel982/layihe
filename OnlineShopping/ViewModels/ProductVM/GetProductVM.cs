using OnlineShopping.Models;

namespace OnlineShopping.ViewModels.ProductVM
{
    public class GetProductVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Image { get; set; }
        public string CategoryName { get; set; }
    }
}
