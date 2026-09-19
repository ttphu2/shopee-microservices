using Infrastructure.Abstractions.Messaging;
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
        List<CreateProductSkuCommand> Skus
    ) : ICommand<CreateProductResult>;

    public record CreateProductSkuCommand(
        string SkuCode,
        decimal Price,
        string? ImageUrl,
        IReadOnlyCollection<SkuAttribute> Attributes
    );
}

