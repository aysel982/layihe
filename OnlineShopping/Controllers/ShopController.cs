using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShopping.DAL;
using OnlineShopping.Models;
using OnlineShopping.Utilities.Enums;
using OnlineShopping.ViewModels;

namespace OnlineShopping.Controllers
{
    public class ShopController : Controller
    {
        private readonly AppDbContext _context;

        public ShopController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(string? search,int? categoryId,int key=1)
        {
            IQueryable<Product> query = _context.Products;
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.Name.ToLower().Contains(search.ToLower()));
            }
            if (categoryId != null || categoryId > 0)
            {
                query = query.Where(p => p.CategoryId == categoryId);
            }

            switch (key)
            {
                case (int)SortType.Name:
                    query = query.OrderBy(p => p.Name);
                    break;
                case (int)SortType.Price:
                    query = query.OrderByDescending(p => p.Price);
                    break;
                case (int)SortType.Date:
                    query = query.OrderByDescending(p => p.CreatedAt);
                    break;
            }
            ShopVM shopVM = new ShopVM
            {
                Products = query.ToList(),
                Categories = await _context.Categories.Include(c=>c.Products).ToListAsync(),
                Search=search,
                CategoryId=categoryId,
                Key=key
            };
            return View(shopVM);
        }
    }
}
