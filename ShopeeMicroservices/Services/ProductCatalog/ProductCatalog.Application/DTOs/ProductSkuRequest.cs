using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Application.DTOs
{
    public record ProductSkuRequest(
        string SkuCode,
        decimal Price,
        string? ImageUrl,
        IReadOnlyCollection<SkuAttribute> Attributes
    );
}
