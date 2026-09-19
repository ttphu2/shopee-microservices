using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ProductCatalog.Domain.Entities;
using ProductCatalog.Domain.Repositories;
using ProductCatalog.Infrastructure.Settings;

namespace ProductCatalog.Infrastructure.Repositories
{
    public class ProductCatalogRepository : IProductCatalogRepository
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<ProductBrand> _brands;
        private readonly IMongoCollection<ProductType> _types;
        private readonly IMongoCollection<ProductSpu> _products;
        private readonly IMongoCollection<ProductSku> _skus;
        public ProductCatalogRepository(IOptions<DatabaseSettings> options)
        {
            var settings = options.Value;
            var client = new MongoClient(settings.ConnectionString);
            _database = client.GetDatabase(settings.DatabaseName);
            _brands = _database.GetCollection<ProductBrand>(settings.BrandCollectionName);
            _types = _database.GetCollection<ProductType>(settings.TypeCollectionName);
            _products = _database.GetCollection<ProductSpu>(settings.ProductSpuCollectionName);
            _skus = _database.GetCollection<ProductSku>(settings.ProductSkusCollectionName);
        }

        public async Task<ProductSpu> CreateProductAsync(ProductSpu product, CancellationToken cancellationToken = default)
        {
            await _products.InsertOneAsync(product, cancellationToken: cancellationToken);
            return product;
        }

        public async Task CreateSkusAsync(IReadOnlyCollection<ProductSku> skus)
        {
            if (skus.Count == 0)
            {
                return;
            }

            await _skus.InsertManyAsync(skus);
        }

        public async Task<IEnumerable<ProductSpu>> GetAllAsync()
        {
            return await _products.Find(p => true).ToListAsync();
        }

        public async Task<ProductSpu> GetProductAsync(string productId)
        {
            return await _products.Find(p => p.Id == productId).FirstOrDefaultAsync();
        }

        public async Task<ProductBrand> GetBrandByIdAsync(string brandId)
        {
            return await _brands.Find(b => b.Id == brandId).FirstOrDefaultAsync();
        }

        public async Task<ProductType> GetCategoryByIdAsync(string typeId)
        {
            return await _types.Find(t => t.Id == typeId).FirstOrDefaultAsync();
        }
    }
}
