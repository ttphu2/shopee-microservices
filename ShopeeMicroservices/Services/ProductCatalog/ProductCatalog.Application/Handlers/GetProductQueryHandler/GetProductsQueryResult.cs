using ProductCatalog.Application.DTOs;

namespace ProductCatalog.Application.Handlers.GetProductQueryHandler
{
    public record GetProductsQueryResult(IList<ProductSpuDto> Products);
}
