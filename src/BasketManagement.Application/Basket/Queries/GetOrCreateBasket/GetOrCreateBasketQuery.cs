using BasketManagement.Application.Common;
using BasketManagement.Application.Basket.DTOs;

namespace BasketManagement.Application.Basket.Queries.GetOrCreateBasket;

public record GetOrCreateBasketQuery(long UserId) : IQuery<ServiceResult<BasketDTO>>;