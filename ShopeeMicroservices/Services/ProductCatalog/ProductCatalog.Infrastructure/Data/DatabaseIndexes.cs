using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ProductCatalog.Domain.Entities;
using ProductCatalog.Infrastructure.Settings;

namespace ProductCatalog.Infrastructure.Data
{
    public static class DatabaseIndexes
    {
        // Idempotent: MongoDB ignores a CreateIndex call for an identical existing index.
        public static async Task EnsureAsync(
            IMongoClient client,
            IOptions<DatabaseSettings> options,
            CancellationToken cancellationToken = default)
        {
            var settings = options.Value;

            var skus = client
                .GetDatabase(settings.DatabaseName)
                .GetCollection<ProductSku>(settings.ProductSkusCollectionName);

            // Backs ProductCatalogRepository.GetSkusByProductIdAsync.
            await skus.Indexes.CreateOneAsync(
                new CreateIndexModel<ProductSku>(
                    Builders<ProductSku>.IndexKeys.Ascending(sku => sku.SpuId)),
                cancellationToken: cancellationToken);

            // SKU codes are unique across all products. The create and update handlers check this first to return a
            // descriptive error; the index guarantees it when concurrent requests pass that check at the same time.
            await skus.Indexes.CreateOneAsync(
                new CreateIndexModel<ProductSku>(
                    Builders<ProductSku>.IndexKeys.Ascending(sku => sku.SkuCode),
                    new CreateIndexOptions { Unique = true }),
                cancellationToken: cancellationToken);
        }
    }
}
