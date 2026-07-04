using BasketManagement.Application.Common;
using BasketManagement.Application.Common.Interfaces;
using BasketManagement.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace BasketManagement.Application.Basket.Commands.ClearBasket;

public class ClearBasketCommandHandler(
    IBasketRepository basketRepository,
    IBasketCacheService cacheService,
    ILogger<ClearBasketCommandHandler> logger) :
    ICommandHandler<ClearBasketCommand, ServiceResult>, IScopedDependency
{
    private readonly IBasketRepository _basketRepository = basketRepository;
    private readonly IBasketCacheService _cacheService = cacheService;
    private readonly ILogger<ClearBasketCommandHandler> _logger = logger;

    public async Task<ServiceResult> Handle(ClearBasketCommand request, CancellationToken cancellationToken)
    {
        var basket = await _basketRepository.GetActiveBasketByUserIdAsync(request.UserId, cancellationToken);
        if (basket is null)
            return ServiceResult.Failure("سبد خرید یافت نشد");

        basket.Clear();
        await _cacheService.RemoveBasketAsync(request.UserId);
        _logger.LogInformation("سبد خرید {basketId} خالی شد", basket.Id);

        return ServiceResult.Success();
    }
}
