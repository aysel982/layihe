using OnlineShopping.ViewModels;

namespace OnlineShopping.Services.Interfaces
{
    public interface IBasketService
    {
        Task<List<BasketItemVM>> GetBasketAsync();
    }
}
