namespace Basket.Application.Abstractions.Discount;

public interface IDiscountService
{
    // Returns the coupon amount per unit for the SKU, or 0 when it has no coupon or the Discount service is unavailable.
    Task<decimal> GetDiscountAmountAsync(string productId, string skuCode, CancellationToken cancellationToken = default);
}
