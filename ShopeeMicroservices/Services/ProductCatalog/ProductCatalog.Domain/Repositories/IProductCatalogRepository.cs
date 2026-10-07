using ProductCatalog.Domain.Entities;
using ProductCatalog.Domain.Products;

namespace ProductCatalog.Domain.Repositories
{
    public interface IProductCatalogRepository
    {
        Task<IEnumerable<ProductSpu>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<ProductDetails?> GetProductWithSkusAsync(string productId, CancellationToken cancellationToken = default);
        Task<ProductSpu> CreateProductAsync(ProductSpu product, CancellationToken cancellationToken = default);
        Task CreateSkusAsync(IReadOnlyCollection<ProductSku> skus, CancellationToken cancellationToken = default);
        Task<ProductBrand> GetBrandByIdAsync(string brandId, CancellationToken cancellationToken = default);
        Task<ProductType> GetCategoryByIdAsync(string typeId, CancellationToken cancellationToken = default);
        Task<ProductBrand?> GetBrandByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<ProductType?> GetCategoryByNameAsync(string name, CancellationToken cancellationToken = default);
    }
}
