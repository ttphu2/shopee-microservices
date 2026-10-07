using ProductCatalog.Domain.Products;

namespace ProductCatalog.Application.Handlers.GetProductByIdQueryHandler
{
    public record GetProductByIdQueryResult(ProductDetails ProductDetails);
}
