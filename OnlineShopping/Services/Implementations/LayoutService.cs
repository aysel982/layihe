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
