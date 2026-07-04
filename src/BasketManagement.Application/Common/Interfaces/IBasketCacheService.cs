using BasketManagement.Application.Basket.DTOs;

namespace BasketManagement.Application.Interfaces;

public interface IBasketCacheService
{
    Task<BasketDTO?> GetBasketAsync(long userId);
    Task SetBasketAsync(long userId, BasketDTO basket, TimeSpan? expiry = null);
    Task RemoveBasketAsync(long userId);
}