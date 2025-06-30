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
        private readonly EmailService _emailService;

        public HomeController(AppDbContext context,EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }
        public async Task<IActionResult> Index()
        {
            await _emailService.SendMailAsync();
            HomeVM vm = new HomeVM
            {
                Products= await _context.Products.Include(p=>p.Category).ToListAsync(),
                Categories= await _context.Categories.ToListAsync()
            };
            return View(vm);
        }
    }
}
