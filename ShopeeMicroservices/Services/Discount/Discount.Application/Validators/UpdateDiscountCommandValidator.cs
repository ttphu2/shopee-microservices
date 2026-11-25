using Discount.Application.Handlers.UpdateDiscountCommandHandler;
using FluentValidation;

namespace Discount.Application.Validators
{
    public sealed class UpdateDiscountCommandValidator : AbstractValidator<UpdateDiscountCommand>
    {
        public UpdateDiscountCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
            RuleFor(x => x.ProductId).NotEmpty();
            RuleFor(x => x.SkuCode).NotEmpty();
            RuleFor(x => x.Description).NotEmpty();
            RuleFor(x => x.Amount).GreaterThan(0);
        }
    }
}
