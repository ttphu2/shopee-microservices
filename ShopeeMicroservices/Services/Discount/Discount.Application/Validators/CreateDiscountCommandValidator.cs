using Discount.Application.Handlers.CreateDiscountCommandHandler;
using FluentValidation;

namespace Discount.Application.Validators
{
    public sealed class CreateDiscountCommandValidator : AbstractValidator<CreateDiscountCommand>
    {
        public CreateDiscountCommandValidator()
        {
            RuleFor(x => x.ProductId).NotEmpty();
            RuleFor(x => x.SkuCode).NotEmpty();
            RuleFor(x => x.Description).NotEmpty();
            RuleFor(x => x.Amount).GreaterThan(0);
        }
    }
}
