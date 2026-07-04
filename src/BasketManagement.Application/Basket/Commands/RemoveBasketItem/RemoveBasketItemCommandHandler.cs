using BasketManagement.Application.Common;
using BasketManagement.Application.Common.Interfaces;
using BasketManagement.Application.Interfaces;

namespace BasketManagement.Application.Basket.Commands.RemoveBasketItem;

public class RemoveBasketItemHandler(IBasketRepository repository):
    ICommandHandler<RemoveBasketItemCommand, ServiceResult>, IScopedDependency
{
    private readonly IBasketRepository _repository = repository;

    public async Task<ServiceResult> Handle(RemoveBasketItemCommand request, CancellationToken ct)
    {
        var basket = await _repository.GetActiveBasketByUserIdAsync(request.UserId, ct);
        if (basket is null)
            return ServiceResult.Failure("سبد خرید یافت نشد");

        basket.RemoveItem(request.ProductId);

        return ServiceResult.Success();
    }
}