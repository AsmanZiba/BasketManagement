using BasketManagement.Application.Basket.DTOs;
using BasketManagement.Application.Common.Interfaces;
using BasketManagement.Application.Interfaces;
using BasketManagement.Domain.Entities;
using BasketManagement.Domain.Enums;
using BasketManagement.Infrastructure.Services.MessageBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BasketManagement.Infrastructure.Persistence.Repositories;

public class BasketRepository : Repository<Basket>, IBasketRepository, IScopedDependency
{
    private readonly RabbitMQSettings basketSetting;

    public BasketRepository(AppDbContext context, IConfiguration configuration) : base(context)
    {
        basketSetting = configuration.GetSection("RabbitMQ").Get<RabbitMQSettings>() ?? new RabbitMQSettings();
    }

    public async Task<Basket?> GetActiveBasketByUserIdAsync(
        long userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(b => b.Items)
            .FirstOrDefaultAsync(b => b.UserId == userId &&
                                      b.Status == Domain.Enums.BasketStatus.Active,
                                      cancellationToken);
    }

    public async Task<List<Basket>> GetExpiredBasketsAsync(CancellationToken cancellationToken = default)
    {
        var threshold = DateTime.UtcNow.AddMinutes(-basketSetting.ExpirationMinutes);

        return await _dbSet
            .Include(b => b.Items)
            .Where(b => b.Status == BasketStatus.Active && b.LastUpdatedAt < threshold)
            .ToListAsync(cancellationToken);
    }
}