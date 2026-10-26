using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Domain.Repositories
{
    public interface IProductCatalogRepository
    {
        Task<IEnumerable<ProductSpu>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<ProductSpu?> GetProductByIdAsync(string productId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<ProductSku>> GetSkusByProductIdAsync(string productId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<ProductSku>> GetSkusByCodesAsync(IReadOnlyCollection<string> skuCodes, CancellationToken cancellationToken = default);
        Task<ProductSpu> CreateProductAsync(ProductSpu product, CancellationToken cancellationToken = default);
        Task CreateSkusAsync(IReadOnlyCollection<ProductSku> skus, CancellationToken cancellationToken = default);
        Task UpdateProductAsync(ProductSpu product, CancellationToken cancellationToken = default);
        Task UpdateSkusAsync(IReadOnlyCollection<ProductSku> skus, CancellationToken cancellationToken = default);
        Task DeleteProductAsync(string productId, CancellationToken cancellationToken = default);
        Task DeleteSkusAsync(IReadOnlyCollection<string> skuIds, CancellationToken cancellationToken = default);
        Task DeleteSkusByProductIdAsync(string productId, CancellationToken cancellationToken = default);
        Task<ProductBrand?> GetBrandByIdAsync(string brandId, CancellationToken cancellationToken = default);
        Task<ProductType?> GetCategoryByIdAsync(string typeId, CancellationToken cancellationToken = default);
        Task<ProductBrand?> GetBrandByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<ProductType?> GetCategoryByNameAsync(string name, CancellationToken cancellationToken = default);
    }
}
