using BasketManagement.Application.Common;
using BasketManagement.Application.Common.Interfaces;
using BasketManagement.Application.Interfaces;
using BasketManagement.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace BasketManagement.Application.Basket.Commands.ExpireBaskets;

public class ExpireBasketsCommandHandler(
    IBasketRepository basketRepository,
    IUnitOfWork unitOfWork,
    IBasketCacheService cacheService,
    IBasketEventPublisher eventPublisher,
    ILogger<ExpireBasketsCommandHandler> logger) :
    ICommandHandler<ExpireBasketsCommand, ServiceResult>, IScopedDependency
{
    private readonly IBasketRepository _basketRepository = basketRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IBasketCacheService _cacheService = cacheService;
    private readonly IBasketEventPublisher _eventPublisher = eventPublisher;
    private readonly ILogger<ExpireBasketsCommandHandler> _logger = logger;

    public async Task<ServiceResult> Handle(ExpireBasketsCommand request, CancellationToken cancellationToken)
    {
        // 1. دریافت سبدهای منقضی شده
        var expiredBaskets = await _basketRepository.GetExpiredBasketsAsync(cancellationToken);

        if (!expiredBaskets.Any())
        {
            _logger.LogInformation("هیچ سبد منقضی شده‌ای یافت نشد");
            return ServiceResult.Success();
        }

        _logger.LogInformation("{Count} سبد منقضی شده یافت شد", expiredBaskets.Count);

        // 2. انقضای هر سبد
        foreach (var basket in expiredBaskets)
        {
            basket.Expire();
            _basketRepository.Update(basket);

            // 3. حذف کش سبد
            await _cacheService.RemoveBasketAsync(basket.UserId);

            // 4. انتشار رویداد انقضای سبد
            await _eventPublisher.PublishBasketExpiredAsync(basket.Id, basket.UserId);

            _logger.LogInformation(
                "سبد {BasketId} کاربر {UserId} منقضی شد",
                basket.Id,
                basket.UserId);
        }

        return ServiceResult.Success();
    }
}