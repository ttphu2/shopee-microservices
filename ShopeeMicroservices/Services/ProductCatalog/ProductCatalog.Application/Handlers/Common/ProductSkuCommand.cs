using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Application.Handlers.Common
{
    public record ProductSkuCommand(
        string SkuCode,
        decimal Price,
        string? ImageUrl,
        IReadOnlyCollection<SkuAttribute> Attributes
    );
}
