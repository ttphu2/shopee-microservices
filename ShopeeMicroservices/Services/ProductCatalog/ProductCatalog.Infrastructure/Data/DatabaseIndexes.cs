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

            // Backs the SKU code conflict check in the create and update handlers (GetSkusByCodesAsync).
            // Not unique yet: existing databases may already contain duplicate codes, which must be cleaned up first.
            await skus.Indexes.CreateOneAsync(
                new CreateIndexModel<ProductSku>(
                    Builders<ProductSku>.IndexKeys.Ascending(sku => sku.SkuCode)),
                cancellationToken: cancellationToken);
        }
    }
}
