using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using BasketManagement.Application.Basket.DTOs;
using BasketManagement.Application.Interfaces;
using BasketManagement.Application.Common.Interfaces;

namespace BasketManagement.Infrastructure.Services.Redis;

public class RedisCacheService : IBasketCacheService, ISingletonDependency
{
    private readonly IDistributedCache _cache;
    private static readonly TimeSpan DefaultExpiry = TimeSpan.FromMinutes(5);

    public RedisCacheService(IDistributedCache cache) => _cache = cache;

    public async Task<BasketDTO?> GetBasketAsync(long userId)
    {
        var key = $"basket:{userId}";
        var data = await _cache.GetStringAsync(key);
        return data == null ? null : JsonSerializer.Deserialize<BasketDTO>(data);
    }

    public async Task SetBasketAsync(long userId, BasketDTO basket, TimeSpan? expiry = null)
    {
        var key = $"basket:{userId}";
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiry ?? DefaultExpiry
        };
        await _cache.SetStringAsync(key, JsonSerializer.Serialize(basket), options);
    }

    public async Task RemoveBasketAsync(long userId)
    {
        var key = $"basket:{userId}";
        await _cache.RemoveAsync(key);
    }
}