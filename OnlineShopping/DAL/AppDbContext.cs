using Microsoft.EntityFrameworkCore;
using OnlineShopping.Models;

namespace OnlineShopping.DAL
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
    }
   
}
