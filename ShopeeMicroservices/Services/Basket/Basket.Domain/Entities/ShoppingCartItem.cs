namespace Basket.Domain.Entities;

// A SKU in the basket. ProductId and SkuCode refer to ProductCatalog's ProductSpu.Id and ProductSku.SkuCode.
public class ShoppingCartItem
{
    public string ProductId { get; set; } = default!;

    public string SkuCode { get; set; } = default!;

    public string ProductName { get; set; } = default!;

    public decimal Price { get; set; }

    public int Quantity { get; set; }
}
