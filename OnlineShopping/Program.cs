using Microsoft.EntityFrameworkCore;
using OnlineShopping.DAL;

namespace OnlineShopping
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<AppDbContext>(opt => {
                opt.UseSqlServer(builder.Configuration.GetConnectionString("default"));
            });
            var app = builder.Build();
            app.UseStaticFiles();
            app.MapControllerRoute(
                "default",
                "{area:exists}/{controller=home}/{action=index}/{id?}"
                );
            app.MapControllerRoute(
                "default",
                "{controller=home}/{action=index}/{id?}"
                );

            app.Run();
        }
    }
}
