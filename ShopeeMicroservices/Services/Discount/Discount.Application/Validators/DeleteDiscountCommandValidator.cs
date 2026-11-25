using Discount.Application.Handlers.DeleteDiscountCommandHandler;
using FluentValidation;

namespace Discount.Application.Validators
{
    public sealed class DeleteDiscountCommandValidator : AbstractValidator<DeleteDiscountCommand>
    {
        public DeleteDiscountCommandValidator()
        {
            RuleFor(x => x.Key).NotNull().SetValidator(new CouponKeyValidator());
        }
    }
}
