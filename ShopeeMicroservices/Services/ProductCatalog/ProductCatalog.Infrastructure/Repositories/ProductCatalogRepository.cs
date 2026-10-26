using System.Linq.Expressions;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ProductCatalog.Domain.Entities;
using ProductCatalog.Domain.Repositories;
using ProductCatalog.Infrastructure.Settings;

namespace ProductCatalog.Infrastructure.Repositories
{
    public class ProductCatalogRepository : IProductCatalogRepository
    {
        private readonly IMongoSessionAccessor _sessionAccessor;
        private readonly IMongoCollection<ProductBrand> _brands;
        private readonly IMongoCollection<ProductType> _types;
        private readonly IMongoCollection<ProductSpu> _products;
        private readonly IMongoCollection<ProductSku> _skus;
        public ProductCatalogRepository(
            IMongoClient client,
            IMongoSessionAccessor sessionAccessor,
            IOptions<DatabaseSettings> options)
        {
            _sessionAccessor = sessionAccessor;
            var settings = options.Value;
            var database = client.GetDatabase(settings.DatabaseName);
            _brands = database.GetCollection<ProductBrand>(settings.BrandCollectionName);
            _types = database.GetCollection<ProductType>(settings.TypeCollectionName);
            _products = database.GetCollection<ProductSpu>(settings.ProductSpuCollectionName);
            _skus = database.GetCollection<ProductSku>(settings.ProductSkusCollectionName);
        }

        // Writes must be part of the unit of work's transaction, so SPU and SKU changes commit or roll back together.
        private IClientSessionHandle WriteSession =>
            _sessionAccessor.CurrentSession
            ?? throw new InvalidOperationException("Repository writes must run inside a unit of work transaction.");

        public async Task<ProductSpu> CreateProductAsync(ProductSpu product, CancellationToken cancellationToken = default)
        {
            await _products.InsertOneAsync(WriteSession, product, cancellationToken: cancellationToken);
            return product;
        }

        public async Task CreateSkusAsync(IReadOnlyCollection<ProductSku> skus, CancellationToken cancellationToken = default)
        {
            if (skus.Count == 0)
            {
                return;
            }

            await _skus.InsertManyAsync(WriteSession, skus, cancellationToken: cancellationToken);
        }

        public async Task UpdateProductAsync(ProductSpu product, CancellationToken cancellationToken = default)
        {
            await _products.ReplaceOneAsync(WriteSession, p => p.Id == product.Id, product, cancellationToken: cancellationToken);
        }

        public async Task UpdateSkusAsync(IReadOnlyCollection<ProductSku> skus, CancellationToken cancellationToken = default)
        {
            if (skus.Count == 0)
            {
                return;
            }

            var replacements = skus
                .Select(sku => new ReplaceOneModel<ProductSku>(Builders<ProductSku>.Filter.Eq(x => x.Id, sku.Id), sku))
                .ToList();

            await _skus.BulkWriteAsync(WriteSession, replacements, cancellationToken: cancellationToken);
        }

        public async Task DeleteProductAsync(string productId, CancellationToken cancellationToken = default)
        {
            await _products.DeleteOneAsync(WriteSession, p => p.Id == productId, cancellationToken: cancellationToken);
        }

        public async Task DeleteSkusAsync(IReadOnlyCollection<string> skuIds, CancellationToken cancellationToken = default)
        {
            if (skuIds.Count == 0)
            {
                return;
            }

            await _skus.DeleteManyAsync(WriteSession, sku => skuIds.Contains(sku.Id), cancellationToken: cancellationToken);
        }

        public async Task DeleteSkusByProductIdAsync(string productId, CancellationToken cancellationToken = default)
        {
            await _skus.DeleteManyAsync(WriteSession, sku => sku.SpuId == productId, cancellationToken: cancellationToken);
        }

        public async Task<IEnumerable<ProductSpu>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _products.Find(p => true).ToListAsync();
        }

        public async Task<ProductSpu?> GetProductByIdAsync(string productId, CancellationToken cancellationToken = default)
        {
            return await Find(_products, p => p.Id == productId).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<ProductSku>> GetSkusByProductIdAsync(string productId, CancellationToken cancellationToken = default)
        {
            // Uses the SpuId index; Ids are GUID v7, so sorting by Id gives creation order.
            return await Find(_skus, sku => sku.SpuId == productId)
                .SortBy(sku => sku.Id)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<ProductSku>> GetSkusByCodesAsync(IReadOnlyCollection<string> skuCodes, CancellationToken cancellationToken = default)
        {
            if (skuCodes.Count == 0)
            {
                return [];
            }

            return await Find(_skus, sku => skuCodes.Contains(sku.SkuCode)).ToListAsync(cancellationToken);
        }

        public async Task<ProductBrand?> GetBrandByIdAsync(string brandId, CancellationToken cancellationToken = default)
        {
            return await Find(_brands, b => b.Id == brandId).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<ProductType?> GetCategoryByIdAsync(string typeId, CancellationToken cancellationToken = default)
        {
            return await Find(_types, t => t.Id == typeId).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<ProductBrand?> GetBrandByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _brands
                .Find(x => x.Name == name)
                .FirstOrDefaultAsync();
        }

        public async Task<ProductType?> GetCategoryByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _types
                .Find(x => x.Name == name)
                .FirstOrDefaultAsync();
        }

        // Reads join the active transaction when there is one (commands) and run without a session otherwise (queries).
        private IFindFluent<T, T> Find<T>(IMongoCollection<T> collection, Expression<Func<T, bool>> filter)
        {
            var session = _sessionAccessor.CurrentSession;
            return session is null ? collection.Find(filter) : collection.Find(session, filter);
        }
    }
}
