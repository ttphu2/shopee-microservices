using Infrastructure.Abstractions.Messaging;
using ProductCatalog.Application.Handlers.Common;
using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Application.Handlers.UpdateProductCommandHandler
{
    // Replaces the product body and its SKU set. SKUs are matched to existing ones by SkuCode.
    public record UpdateProductCommand(
        string Id,
        string Name,
        string Description,
        string BrandId,
        string CategoryId,
        List<string> Images,
        List<VariantOption> VariantOptions,
        List<ProductSkuCommand> Skus
    ) : ICommand, IProductCommand;
}
