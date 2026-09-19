using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Application.DTOs
{
    public record class CreateProductRequest
    {
        public required string ProductCode { get; init; }
        public required string Name { get; init; }
        public required string Description { get; init; }
        public required string BrandId { get; init; }
        public required string CategoryId { get; init; }

        public List<string> Images { get; init; } = [];
        public List<VariantOption> VariantOptions { get; init; } = [];
        public List<CreateProductSkuRequest> Skus { get; init; } = [];
    }

    public record CreateProductSkuRequest(
        string SkuCode,
        decimal Price,
        string? ImageUrl,
        IReadOnlyCollection<SkuAttribute> Attributes
    );
}
