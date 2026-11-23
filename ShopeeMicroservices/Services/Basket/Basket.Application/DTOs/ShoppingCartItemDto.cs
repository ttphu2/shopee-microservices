namespace Basket.Application.DTOs
{
    // ProductId and SkuCode refer to ProductCatalog's ProductSpu.Id and ProductSku.SkuCode (not checked here).
    public sealed record ShoppingCartItemDto(
        string ProductId,
        string SkuCode,
        string ProductName,
        decimal Price,
        int Quantity
    );
}
