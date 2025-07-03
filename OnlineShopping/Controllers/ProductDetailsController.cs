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
            //ProductDetailVM detailVM=new ProductDetailVM
            //{
            //    Product=
            //};
            //return View(detailVM);
            return View();
        }
        public IActionResult Detail(int? id)
        {
            if(id is null|| id <= 0)
            {
                return BadRequest();
            }
            Product? product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product is null) return NotFound();

            ProductDetailVM detailVM = new ProductDetailVM
            {
                Product = product,
            };
            return View(detailVM);
        }
    }
}
