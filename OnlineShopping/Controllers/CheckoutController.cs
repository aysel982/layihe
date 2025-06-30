using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShopping.DAL;
using OnlineShopping.Models;
using OnlineShopping.Services.Interfaces;
using OnlineShopping.ViewModels;
using System.Security.Claims;

namespace OnlineShopping.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;

        public CheckoutController(AppDbContext context,IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }
        public IActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "member")]
        public async Task<IActionResult> Checkout()
        {
            OrderVM orderVM = new OrderVM
            {
                BasketInOrderVMs = await _context.BasketItems
                .Where(bi => bi.AppUserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                .Select(bi => new BasketInOrderVM
                {
                    Count = bi.Count,
                    Name = bi.Product.Name,
                    Price = bi.Product.Price,
                    Subtotal = bi.Product.Price * bi.Count
                }).ToListAsync()
            };
            return View(orderVM);
        }
        [HttpPost]
        public async Task<IActionResult> Checkout(OrderVM orderVM)
        {
            List<BasketItem> basketItems = await _context.BasketItems
                .Where(bi => bi.AppUserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                .Include(bi=>bi.Product)
                .ToListAsync();

            if (!ModelState.IsValid)
            {
                orderVM.BasketInOrderVMs = basketItems.Select(bi => new BasketInOrderVM
                {
                    Count = bi.Count,
                    Name = bi.Product.Name,
                    Price = bi.Product.Price,
                    Subtotal = bi.Product.Price * bi.Count
                }).ToList();
                return View(orderVM);
            }

            Order order = new Order
            {
                Adress = orderVM.Adress,
                Status = null,
                AppUserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                CreatedAt = DateTime.Now,
                OrderItems = basketItems.Select(bi => new OrderItem
                {
                    AppUserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    Count = bi.Count,
                    Price = bi.Product.Price,
                    ProductId = bi.ProductId

                }).ToList(),
                TotalPrice = basketItems.Sum(bi => bi.Product.Price * bi.Count)
            };

            await _context.Orders.AddAsync(order);
            _context.BasketItems.RemoveRange(basketItems);
            await _context.SaveChangesAsync();

            //await _emailService.SendMailAsync(user.Email, "your order", body, true);

            return RedirectToAction("Index", "Home");
        }
    }
}
