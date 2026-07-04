using FluentValidation;

namespace BasketManagement.Application.Basket.Commands.AddItemToBasket;

public class AddItemToBasketCommandValidator : AbstractValidator<AddItemToBasketCommand>
{
    public AddItemToBasketCommandValidator()
    {
        RuleFor(x => x.UserId).GreaterThan(0).WithMessage("شناسه کاربر نامعتبر است.");
        RuleFor(x => x.Item.ProductId).GreaterThan(0).WithMessage("شناسه کالا باید بزرگتر از صفر باشد.");
        RuleFor(x => x.Item.Quantity).InclusiveBetween(1, 10).WithMessage("تعداد باید بین ۱ تا ۱۰ باشد.");
    }
}