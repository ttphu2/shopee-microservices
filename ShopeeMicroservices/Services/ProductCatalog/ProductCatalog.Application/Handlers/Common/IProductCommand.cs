using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Application.Handlers.Common
{
    // Product body shared by the create and update commands, so both are validated by the same rules.
    public interface IProductCommand
    {
        string Name { get; }
        string Description { get; }
        string BrandId { get; }
        string CategoryId { get; }
        List<string> Images { get; }
        List<VariantOption> VariantOptions { get; }
        List<ProductSkuCommand> Skus { get; }
    }
}
