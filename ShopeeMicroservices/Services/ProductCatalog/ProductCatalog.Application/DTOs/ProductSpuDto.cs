namespace ProductCatalog.Application.DTOs
{
    public sealed record ProductSpuDto(
        string Id,
        string ProductCode,
        string Name,
        string Description,
        string BrandId,
        string CategoryId,
        IReadOnlyList<string> Images,
        IReadOnlyList<VariantOptionDto> VariantOptions,
        DateTime? CreatedDate,
        string? CreatedBy,
        DateTime? LastModified,
        string? LastModifiedBy
    );

    public sealed record VariantOptionDto(
        string Name,
        string Code,
        IReadOnlyList<string> Values
    );
}
