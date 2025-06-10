using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShopping.DAL;
using OnlineShopping.ViewModels;

namespace OnlineShopping.Controllers
{
    public class NewDetailsController : Controller
    {
        private readonly AppDbContext _context;

        public NewDetailsController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> IndexAsync()
        {
            HomeVM vm = new HomeVM
            {
                Products = await _context.Products.ToListAsync(),
                Categories = await _context.Categories.ToListAsync()
            };
            return View(vm);
        }
    }
}
