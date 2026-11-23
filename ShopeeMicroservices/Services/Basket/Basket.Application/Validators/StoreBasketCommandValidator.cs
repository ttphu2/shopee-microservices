using Basket.Application.Handlers.StoreBasketCommandHandler;
using Basket.Application.Resources;
using FluentValidation;

namespace Basket.Application.Validators
{
    public sealed class StoreBasketCommandValidator : AbstractValidator<StoreBasketCommand>
    {
        public StoreBasketCommandValidator()
        {
            RuleFor(x => x.UserName).NotEmpty();
            RuleFor(x => x.Items).NotNull();

            RuleForEach(x => x.Items).ChildRules(item =>
            {
                item.RuleFor(i => i.ProductId).NotEmpty();
                item.RuleFor(i => i.SkuCode).NotEmpty();
                item.RuleFor(i => i.ProductName).NotEmpty();
                item.RuleFor(i => i.Price).GreaterThan(0);
                item.RuleFor(i => i.Quantity).GreaterThan(0);
            });

            RuleFor(x => x.Items)
                .Must(items => items.Select(i => i.SkuCode).Distinct(StringComparer.Ordinal).Count() == items.Count)
                .When(x => x.Items is not null)
                .WithMessage(_ => ServerMessages.SkuCodesNotUnique);
        }
    }
}
