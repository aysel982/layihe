using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShopping.DAL;
using OnlineShopping.Models;
using OnlineShopping.Utilities;
using OnlineShopping.Utilities.Extentions;
using OnlineShopping.ViewModels;
using System.Threading.Tasks;

namespace OnlineShopping.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            List<GetProductVM> productVMs = await _context.Products.Select(p => new GetProductVM
            {
                Name = p.Name,
                Id = p.Id,
                Image = p.Image,
                CategoryName = p.Category.Name,
                Price = p.Price
            }).ToListAsync();
            return View(productVMs);
        }

        public async Task<IActionResult> Create()
        {
            CreateProductVM productVM = new CreateProductVM
            {
                Categories = await _context.Categories.ToListAsync()
            };
            return View(productVM);
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateProductVM productVM)
        {
            productVM.Categories = await _context.Categories.ToListAsync();
            if (!ModelState.IsValid)
            {
                return View(productVM);
            }
            bool result = productVM.Categories.Any(c => c.Id == productVM.CategoryId);
            if (!result)
            {
                ModelState.AddModelError(nameof(CreateProductVM.CategoryId), "category doesn't exists");
                return View(productVM);
            }

            if (!productVM.ProductPhoto.ValidateType("image/"))
            {
                ModelState.AddModelError(nameof(CreateProductVM.ProductPhoto), "Type incorrect");
                return View(productVM);
            }
            if (!productVM.ProductPhoto.ValidateSize(Utilities.FileSize.KB, 500))
            {
                ModelState.AddModelError(nameof(CreateProductVM.ProductPhoto), "Size incorrect");
                return View(productVM);
            }
            string fileName = await productVM.ProductPhoto.CreatefileAsync(_env.WebRootPath, "assets", "images", "products");

            bool Nameresult = await _context.Categories.AnyAsync(p => p.Name == productVM.Name);
            if (Nameresult)
            {
                ModelState.AddModelError(nameof(CreateProductVM.Name), "Already exists");
                return View(productVM);
            }
            Product product = new Product
            {
                Name = productVM.Name,
                Price = productVM.Price.Value,
                Title = productVM.Title,
                Subtitle = productVM.Subtitle,
                CategoryId = productVM.CategoryId.Value,
                Image = fileName
            };
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Update(int? id)
        {
            if (id is null || id <= 0) return BadRequest();
            Product? product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product is null) return NotFound();
            UpdateProductVM productVM = new UpdateProductVM
            {
                Name = product.Name,
                Title = product.Title,
                Subtitle = product.Subtitle,
                Categories = await _context.Categories.ToListAsync(),
                CategoryId = product.CategoryId,
                Price = product.Price,
                PrimaryImage = product.Image
            };
            return View(productVM);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int? id, UpdateProductVM productVM)
        {
            productVM.Categories =  _context.Categories.ToList();

            {
                if (!ModelState.IsValid) return View(productVM);
                
            }
            Product? existed = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (existed is null) return NotFound();

            if (productVM.ProductPhoto is not null)
            {
                if (!productVM.ProductPhoto.ValidateType("image/"))
                {
                    ModelState.AddModelError(nameof(CreateProductVM.ProductPhoto), "Type incorrect");
                    return View(productVM);
                }
                if (!productVM.ProductPhoto.ValidateSize(FileSize.KB, 500))
                {
                    ModelState.AddModelError(nameof(CreateProductVM.ProductPhoto), "Size incorrect");
                    return View(productVM);
                }
                string fileName = await productVM.ProductPhoto.CreatefileAsync(_env.WebRootPath, "assets", "images", "products");
                productVM.PrimaryImage.DeleteFile(_env.WebRootPath, "assets", "images", "products");
                existed.Image = fileName;
            }
            bool result = await _context.Categories.AnyAsync(c => c.Id == productVM.CategoryId);
            if (!result)
            {
                ModelState.AddModelError(nameof(CreateProductVM.Name), "Already exists");
                return View(productVM);
            }
            bool Nameresult = await _context.Categories.AnyAsync(p => p.Name == productVM.Name&&p.Id!=id);
            if (Nameresult)
            {
                ModelState.AddModelError(nameof(CreateProductVM.Name), "Already exists");
                return View(productVM);
            }
            

            existed.Name = productVM.Name;
            existed.Title = productVM.Title;
            existed.Subtitle = productVM.Subtitle;
            existed.CategoryId = productVM.CategoryId.Value;
            existed.Price = productVM.Price.Value;

            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null || id <= 0) return BadRequest();
            Product? product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if(product is null) return NotFound();
            product.Image.DeleteFile(_env.WebRootPath, "assets", "images", "products");
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
