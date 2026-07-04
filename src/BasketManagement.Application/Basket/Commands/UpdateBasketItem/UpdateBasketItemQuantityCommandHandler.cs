using BasketManagement.Application.Basket.DTOs;
using BasketManagement.Application.Common;
using BasketManagement.Application.Common.Interfaces;
using BasketManagement.Application.Interfaces;
using BasketManagement.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace BasketManagement.Application.Basket.Commands.UpdateBasketItem;

public class UpdateBasketItemQuantityCommandHandler(
    IBasketRepository basketRepository,
    IUnitOfWork unitOfWork,
    IBasketCacheService cacheService,
    IBasketEventPublisher eventPublisher,
    ILogger<UpdateBasketItemQuantityCommandHandler> logger) :
    ICommandHandler<UpdateBasketItemQuantityCommand, ServiceResult>, IScopedDependency
{
    private readonly IBasketRepository _basketRepository = basketRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IBasketCacheService _cacheService = cacheService;
    private readonly IBasketEventPublisher _eventPublisher = eventPublisher;
    private readonly ILogger<UpdateBasketItemQuantityCommandHandler> _logger = logger;

    public async Task<ServiceResult> Handle(UpdateBasketItemQuantityCommand request, CancellationToken cancellationToken)
    {
        var basket = await _basketRepository.GetActiveBasketByUserIdAsync(request.UserId, cancellationToken);
        if (basket is null)
            return ServiceResult.Failure("سبد خرید فعالی یافت نشد.");

        decimal unitPrice = 1000m;
        basket.UpdateQuantity(request.ProductId, request.NewQuantity, unitPrice);

        _basketRepository.Update(basket);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _cacheService.SetBasketAsync(request.UserId, BasketMapper.MapToDTO(basket));

        // انتشار رویداد یکپارچه‌سازی
        await _eventPublisher.PublishBasketItemAddedAsync(request.UserId, request.ProductId, request.NewQuantity);

        return ServiceResult.Success();
    }

}