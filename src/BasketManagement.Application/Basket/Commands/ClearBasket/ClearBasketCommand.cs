using BasketManagement.Application.Common;

namespace BasketManagement.Application.Basket.Commands.ClearBasket;

public record ClearBasketCommand(long UserId) : ICommand<ServiceResult>;