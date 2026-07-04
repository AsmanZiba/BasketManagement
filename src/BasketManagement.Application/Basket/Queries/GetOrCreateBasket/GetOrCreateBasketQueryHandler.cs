using BasketManagement.Application.Basket.DTOs;
using BasketManagement.Application.Common;
using BasketManagement.Application.Common.Interfaces;
using BasketManagement.Application.Interfaces;
using BasketManagement.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace BasketManagement.Application.Basket.Queries.GetOrCreateBasket;

public class GetOrCreateBasketQueryHandler(IBasketRepository basketRepository,
    IBasketCacheService cacheService,
    ILogger<GetOrCreateBasketQueryHandler> logger) :
    IQueryHandler<GetOrCreateBasketQuery, ServiceResult<BasketDTO>>, IScopedDependency
{
    private readonly IBasketRepository _basketRepository = basketRepository;
    private readonly IBasketCacheService _cacheService = cacheService;
    private readonly ILogger<GetOrCreateBasketQueryHandler> _logger = logger;

    public async Task<ServiceResult<BasketDTO>> Handle(GetOrCreateBasketQuery request, CancellationToken cancellationToken)
    {
        // ۱. کش
        var cached = await _cacheService.GetBasketAsync(request.UserId);
        if (cached is not null)
            return ServiceResult<BasketDTO>.Success(cached);

        // ۲. دیتابیس
        var basket = await _basketRepository.GetActiveBasketByUserIdAsync(request.UserId, cancellationToken);
        if (basket is null)
        {
            basket = new Domain.Entities.Basket(request.UserId);
            await _basketRepository.AddAsync(basket, cancellationToken);
        }

        var dto = BasketMapper.MapToDTO(basket);
        await _cacheService.SetBasketAsync(request.UserId, dto, TimeSpan.FromMinutes(5));
        _logger.LogInformation("سبد خرید برای {UserId} ایجاد شد.", request.UserId);
        return ServiceResult<BasketDTO>.Success(dto);
    }
}