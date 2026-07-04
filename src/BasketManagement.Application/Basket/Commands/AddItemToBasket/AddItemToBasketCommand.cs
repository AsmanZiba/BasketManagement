using BasketManagement.Application.Common;

namespace BasketManagement.Application.Basket.Commands.AddItemToBasket;

public record AddItemToBasketCommand(long UserId, AddBasketItemDTO Item) : ICommand<ServiceResult>;