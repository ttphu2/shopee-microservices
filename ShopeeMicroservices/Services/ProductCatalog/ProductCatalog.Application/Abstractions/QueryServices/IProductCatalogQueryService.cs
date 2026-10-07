using ProductCatalog.Application.DTOs;

namespace ProductCatalog.Application.QueryServices
{
    public interface IProductCatalogQueryService
    {
        Task<ProductSpuDto?> GetProductDetailsAsync(
            string productId,
            CancellationToken cancellationToken = default);
    }
}
