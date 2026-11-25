using Discount.Domain.ValueObjects;
using FluentValidation;

namespace Discount.Application.Validators
{
    // Reused by every command that addresses a coupon by its key (SetValidator).
    public sealed class CouponKeyValidator : AbstractValidator<CouponKey>
    {
        public CouponKeyValidator()
        {
            RuleFor(x => x.ProductId).NotEmpty();
            RuleFor(x => x.SkuCode).NotEmpty();
        }
    }
}
