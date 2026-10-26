namespace ProductCatalog.Application.DTOs
{
    public sealed record ProductDetailsDto(
        ProductSpuDto Product,
        ProductBrandDto? Brand,
        ProductTypeDto? Type,
        IReadOnlyList<ProductSkuDto> Skus,
        decimal? MinPrice,
        decimal? MaxPrice
    );
}
