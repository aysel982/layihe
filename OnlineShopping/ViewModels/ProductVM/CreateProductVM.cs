using OnlineShopping.Models;
using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.ViewModels
{
    public class CreateProductVM
    {
        public string Name { get; set; }
        [Required]
        public decimal? Price { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        //public string Image { get; set; }
        [Required]
        public int? CategoryId { get; set; }
        public List<int>? TagIds { get; set; }
        public int TagId { get; set; }
        public IFormFile ProductPhoto { get; set; }
        public List<Category>? Categories { get; set; }
        public List<Tag>? Tags { get; set; }

    }
}
