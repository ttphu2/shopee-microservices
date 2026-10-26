using Infrastructure.Abstractions.Messaging;
using ProductCatalog.Application.Handlers.Common;
using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Application.Handlers.CreateProductCommandHandler
{
    public record CreateProductCommand(
        string ProductCode,
        string Name,
        string Description,
        string BrandId,
        string CategoryId,
        List<string> Images,
        List<VariantOption> VariantOptions,
        List<ProductSkuCommand> Skus
    ) : ICommand<CreateProductResult>, IProductCommand;
}

