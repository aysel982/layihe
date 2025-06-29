using OnlineShopping.ViewModels;

namespace OnlineShopping.Services.Interfaces
{
    public interface ILayoutService
    {
        Task<Dictionary<string, string>> GetSettingsAsync();
        Task<List<BasketItemVM>> GetBasketAsync();
    }
}
