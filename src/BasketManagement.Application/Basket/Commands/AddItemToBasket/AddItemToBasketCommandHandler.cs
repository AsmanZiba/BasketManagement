using BasketManagement.Application.Basket.DTOs;
using BasketManagement.Application.Common;
using BasketManagement.Application.Common.Interfaces;
using BasketManagement.Application.Interfaces;
using BasketManagement.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace BasketManagement.Application.Basket.Commands.AddItemToBasket;

public class AddItemToBasketCommandHandler(
    IBasketRepository basketRepository,
    IUnitOfWork unitOfWork,
    IBasketCacheService cacheService,
    IBasketEventPublisher eventPublisher,
    ILogger<AddItemToBasketCommandHandler> logger) : 
    ICommandHandler<AddItemToBasketCommand, ServiceResult>, IScopedDependency
{
    private readonly IBasketRepository _basketRepository = basketRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IBasketCacheService _cacheService = cacheService;
    private readonly IBasketEventPublisher _eventPublisher = eventPublisher;
    private readonly ILogger<AddItemToBasketCommandHandler> _logger = logger;

    public async Task<ServiceResult> Handle(AddItemToBasketCommand request, CancellationToken cancellationToken)
    {
        var cached = await _cacheService.GetBasketAsync(request.UserId);
        if (cached is not null)
            return ServiceResult<BasketDTO>.Success(cached);

        var basket = await _basketRepository.GetActiveBasketByUserIdAsync(request.UserId, cancellationToken);
        if (basket is null)
            return ServiceResult.Failure("سبد خرید فعالی یافت نشد. لطفاً ابتدا سبد را ایجاد کنید.");

        decimal unitPrice = 1000m;
        basket.AddItem(request.Item.ProductId, request.Item.Quantity, unitPrice);

        _basketRepository.Update(basket);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _cacheService.SetBasketAsync(request.UserId, BasketMapper.MapToDTO(basket));
        _logger.LogInformation("کالا با شناسه {ProductId} به سبد {BasketId} اضافه شد",
            request.Item.ProductId, basket.Id);
        // انتشار رویداد یکپارچه‌سازی
        await _eventPublisher.PublishBasketItemAddedAsync(request.UserId, request.Item.ProductId, request.Item.Quantity);

        return ServiceResult.Success();
    }
}