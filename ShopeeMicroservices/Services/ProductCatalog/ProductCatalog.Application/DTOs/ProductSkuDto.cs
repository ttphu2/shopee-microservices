namespace ProductCatalog.Application.DTOs
{
    public sealed record ProductSkuDto(
        string Id,
        string SpuId,
        string SkuCode,
        decimal Price,
        string? ImageUrl,
        IReadOnlyList<SkuAttributeDto> Attributes,
        DateTime? CreatedDate,
        string? CreatedBy,
        DateTime? LastModified,
        string? LastModifiedBy
    );

    public sealed record SkuAttributeDto(
        string AttributeCode,
        string Value
    );
}
