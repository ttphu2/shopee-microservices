using Discount.Domain.Entities;

namespace Discount.Infrastructure.Data
{
    public static class DiscountSeedData
    {
        public static List<Coupon> CreateCoupons() =>
        [
            new() { ProductId = "019a7e00-0000-7000-8000-000000000301", SkuCode = "IPHONE-X-SG-64", Description = "iPhone X Space Gray 64GB discount", Amount = 50m },
            new() { ProductId = "019a7e00-0000-7000-8000-000000000308", SkuCode = "IPHONE-17-BK-256", Description = "iPhone 17 Black 256GB discount", Amount = 100m },
        ];
    }
}
