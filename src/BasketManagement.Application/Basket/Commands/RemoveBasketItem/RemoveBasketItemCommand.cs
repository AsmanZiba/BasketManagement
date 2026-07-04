using BasketManagement.Application.Common;

namespace BasketManagement.Application.Basket.Commands.RemoveBasketItem;

public record RemoveBasketItemCommand(long UserId, long ProductId) : ICommand<ServiceResult>;