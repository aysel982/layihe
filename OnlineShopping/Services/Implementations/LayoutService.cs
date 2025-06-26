using Microsoft.EntityFrameworkCore;
using OnlineShopping.DAL;
using System.Threading.Tasks;

namespace OnlineShopping.Services
{
    public class LayoutService: ILayoutService
    {
        private readonly AppDbContext _context;

        public LayoutService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Dictionary<string, string>> GetSettingsAsync()
        {
            var settings = await _context.Settings.ToDictionaryAsync(s => s.Key, s => s.Value);
                return settings;
        }
    }
}
