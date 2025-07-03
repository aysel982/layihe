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
        public async Task<IActionResult> Index()
        {
            HomeVM vm = new HomeVM
            {
                Products = await _context.Products.Include(p => p.Category).ToListAsync(),
                Categories = await _context.Categories.ToListAsync()
            };
            return View(vm);
        }
       
       
    }
}
