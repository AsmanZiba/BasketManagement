using BasketManagement.Application.Common;
using BasketManagement.Application.Common.Interfaces;
using BasketManagement.Application.Interfaces;

namespace BasketManagement.Application.Basket.Commands.RemoveBasketItem;

public class RemoveBasketItemHandler(IBasketRepository repository, IBasketCacheService cacheService) :
    ICommandHandler<RemoveBasketItemCommand, ServiceResult>, IScopedDependency
{
    private readonly IBasketRepository _repository = repository;
    private readonly IBasketCacheService _cacheService = cacheService;

    public async Task<ServiceResult> Handle(RemoveBasketItemCommand request, CancellationToken ct)
    {
        var basket = await _repository.GetActiveBasketByUserIdAsync(request.UserId, ct);
        if (basket is null)
            return ServiceResult.Failure("سبد خرید یافت نشد");

        basket.RemoveItem(request.ProductId);
        await _cacheService.RemoveBasketAsync(request.UserId);

        return ServiceResult.Success();
    }
}