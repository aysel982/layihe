using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NuGet.Protocol;
using OnlineShopping.DAL;
using OnlineShopping.Models;
using OnlineShopping.Services.Interfaces;
using OnlineShopping.ViewModels;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OnlineShopping.Controllers
{
    public class BasketController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IBasketService _basketService;

        public BasketController(AppDbContext context,UserManager<AppUser>userManager,IBasketService basketService)
        {
            _context = context;
            _userManager = userManager;
            _basketService = basketService;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _basketService.GetBasketAsync());
        }
        public async Task<IActionResult> AddBasket(int? id)
        {
            if (id is null || id <= 0) return BadRequest();
            bool result = await _context.Products.AnyAsync(p => p.Id == id);
            if (!result) NotFound();
            if (User.Identity.IsAuthenticated)
            {
                AppUser? user = await _userManager.Users
                    .Include(u => u.BasketItems)
                    .FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));

                BasketItem item = user.BasketItems.FirstOrDefault(bi => bi.ProductId == id);
                if(item is null)
                {
                    user.BasketItems.Add(new BasketItem
                    {
                        ProductId = id.Value,
                        Count = 1
                    });
                }
                else
                {
                    item.Count++;
                }
                await _context.SaveChangesAsync();
            }
            else
            {
                List<BasketCookieItemVM> basket;
                string cookies = Request.Cookies["Basket"];

                if (cookies != null)
                {
                    basket = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(cookies);
                    BasketCookieItemVM existed = basket.FirstOrDefault(b => b.Id == id);
                    if (existed != null)
                    {
                        existed.Count++;
                    }
                    else
                    {
                        basket.Add(new()
                        {
                            Id = id.Value,
                            Count = 1
                        });
                    }
                }
                else
                {
                    basket = new();
                    basket.Add(new()
                    {
                        Id = id.Value,
                        Count = 1
                    });
                }
                string Json = JsonConvert.SerializeObject(basket);
                Response.Cookies.Append("Basket", Json);
            }
            return RedirectToAction("Index", "Home");
        }
        
       

    }
}
