using OnlineShopping.Models;

namespace OnlineShopping.ViewModels
{
    public class ProductDetailVM
    {
        public Product Product { get; set; }
        public List<Product> RelatedProducts { get; set; } 
    }
}
