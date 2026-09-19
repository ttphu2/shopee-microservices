using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Domain.Repositories
{
    public interface IProductCatalogRepository
    {
        Task<IEnumerable<ProductSpu>> GetAllAsync();
        Task<ProductSpu> CreateProductAsync(ProductSpu product, CancellationToken cancellationToken = default);
        Task CreateSkusAsync(IReadOnlyCollection<ProductSku> skus);
        Task<ProductBrand> GetBrandByIdAsync(string brandId);
        Task<ProductType> GetCategoryByIdAsync(string typeId);
    }
}
