using System.Globalization;
using Discount.Application.Resources;
using Infrastructure.Errors;

namespace Discount.Application.Errors
{
    public static class DiscountErrors
    {
        public const string CouponNotFoundCode = "Discount.CouponNotFound";
        public const string CouponAlreadyExistsCode = "Discount.CouponAlreadyExists";

        public static readonly Error CouponNotFound = new(CouponNotFoundCode, ServerMessages.CouponNotFound);

        public static Error CouponAlreadyExists(string skuCode) =>
            new(CouponAlreadyExistsCode, string.Format(CultureInfo.CurrentCulture, ServerMessages.CouponAlreadyExists, skuCode));
    }
}
