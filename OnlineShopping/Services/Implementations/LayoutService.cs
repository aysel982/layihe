using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OnlineShopping.DAL;
using OnlineShopping.Models;
using OnlineShopping.Services.Interfaces;
using OnlineShopping.ViewModels;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OnlineShopping.Services
{
    public class LayoutService: ILayoutService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _http;
        private readonly ClaimsPrincipal _user;

        public LayoutService(AppDbContext context,IHttpContextAccessor http)
        {
            _context = context;
            _http = http;
            _user = _http.HttpContext.User;
        }

        public async Task<List<BasketItemVM>> GetBasketAsync()
        {
            List<BasketItemVM> basketVM = new();
            if (_user.Identity.IsAuthenticated)
            {
                basketVM = await _context.BasketItems
                    .Where(bi => bi.AppUserId == _user.FindFirstValue(ClaimTypes.NameIdentifier))
                    .Select(bi => new BasketItemVM
                    {
                        Count = bi.Count,
                        Image = bi.Product.Image,
                        Name = bi.Product.Name,
                        Price = bi.Product.Price,
                        Subtotal = bi.Count * bi.Product.Price,
                        Id = bi.ProductId
                    }).ToListAsync();
            }
            else
            {
                List<BasketCookieItemVM> cookiesVM;
                string cookie = _http.HttpContext.Request.Cookies["basket"];


                if (cookie is null)
                {
                    return basketVM;
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
            return basketVM;
        }

        public async Task<Dictionary<string, string>> GetSettingsAsync()
        {
            var settings = await _context.Settings.ToDictionaryAsync(s => s.Key, s => s.Value);
                return settings;
        }
    }
}
