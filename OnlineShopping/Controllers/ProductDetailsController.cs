using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShopping.DAL;
using OnlineShopping.Models;
using OnlineShopping.ViewModels;

namespace OnlineShopping.Controllers
{
    public class ProductDetailsController : Controller
    {
        private readonly AppDbContext _context;

        public ProductDetailsController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Detail(int? id)
        {
            if(id is null|| id <= 0)
            {
                return BadRequest();
            }
            Product? product = _context.Products.Include(p=>p.Image).FirstOrDefault(p => p.Id == p.Id);
            if (product is null) return NotFound();

            ProductDetailVM detailVM = new ProductDetailVM
            {
                Product = product,
                RelatedProducts = _context.Products.Where(p => p.CategoryId == product.CategoryId && p.Id != product.Id).ToList()
            };
            return View(detailVM);
        }
    }
}
