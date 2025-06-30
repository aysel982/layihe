using OnlineShopping.Models;

namespace OnlineShopping.ViewModels
{
    public class ShopVM
    {
        public List<Product> Products { get; set; }
        public List<Category> Categories { get; set; }
        public string Search { get; set; }
        public int? CategoryId { get; set; }
        public int Key { get; set; }
    }
}
