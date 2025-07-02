using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShopping.DAL;
using OnlineShopping.Services.Implementations;
using OnlineShopping.ViewModels;
using System.Threading.Tasks;

namespace OnlineShopping.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
           
        }
        public async Task<IActionResult> Index()
        {
            
            HomeVM vm = new HomeVM
            {
                Products= await _context.Products.Include(p=>p.Category).ToListAsync(),
                Categories= await _context.Categories.ToListAsync()
            };
            return View(vm);
        }
    }
}
