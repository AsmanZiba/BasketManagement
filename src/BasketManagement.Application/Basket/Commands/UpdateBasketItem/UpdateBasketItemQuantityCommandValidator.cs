using FluentValidation;

namespace BasketManagement.Application.Basket.Commands.UpdateBasketItem;

public class UpdateBasketItemQuantityCommandValidator : AbstractValidator<UpdateBasketItemQuantityCommand>
{
    public UpdateBasketItemQuantityCommandValidator()
    {
        RuleFor(x => x.UserId).GreaterThan(0);
        RuleFor(x => x.ProductId).GreaterThan(0);
        RuleFor(x => x.NewQuantity).InclusiveBetween(1, 10);
    }
}