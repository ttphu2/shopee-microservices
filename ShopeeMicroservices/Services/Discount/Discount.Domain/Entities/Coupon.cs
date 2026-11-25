namespace Discount.Domain.Entities;

// A discount for one SKU (the same SkuCode Basket items carry).
public class Coupon
{
    public int Id { get; set; }

    // ProductCatalog product (SPU) id the SKU belongs to; a reference only, not checked against ProductCatalog.
    public string ProductId { get; set; } = default!;

    public string SkuCode { get; set; } = default!;

    public string Description { get; set; } = default!;

    public decimal Amount { get; set; }
}
