using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NuGet.Protocol;
using OnlineShopping.DAL;
using OnlineShopping.Models;
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

        public BasketController(AppDbContext context,UserManager<AppUser>userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            List<BasketItemVM> basketVM = new();
            if (User.Identity.IsAuthenticated)
            {
                basketVM = await _context.BasketItems
                    .Where(bi=>bi.AppUserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                    .Select(bi => new BasketItemVM
                    {
                        Count = bi.Count,
                        Image = bi.Product.Image,
                        Name = bi.Product.Name,
                        Price = bi.Product.Price,
                        Subtotal = bi.Count * bi.Product.Price,
                        Id=bi.ProductId
                    }).ToListAsync();
            }
            else
            {
                List<BasketCookieItemVM> cookiesVM;
                string cookie = Request.Cookies["basket"];


                if (cookie is null)
                {
                    return View(basketVM);
                }

                cookiesVM = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(cookie);
                foreach (BasketCookieItemVM item in cookiesVM)
                {
                    Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == item.Id);
                    if (product is not null)
                    {
                        basketVM.Add(new BasketItemVM
                        {
                            Id = product.Id,
                            Name = product.Name,
                            Price = product.Price,
                            Image = product.Image,
                            Count = item.Count,
                            Subtotal = item.Count * product.Price


                        });
                    }
                }
            }
            return View(basketVM);
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
