using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OnlineShopping.Models
{
    public class Order:BaseEntity
    {
        public string Adress { get; set; }
        public decimal TotalPrice { get; set; }
        public AppUser AppUser { get; set; }
        public string AppUserId { get; set; }
        public List<OrderItem> OrderItems { get; set; }
        public bool? Status { get; set; }
    }
}
