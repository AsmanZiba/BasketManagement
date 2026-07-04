using BasketManagement.Application.Common;

namespace BasketManagement.Application.Basket.Commands.UpdateBasketItem;

public record UpdateBasketItemQuantityCommand(long UserId, long ProductId, int NewQuantity) : ICommand<ServiceResult>;