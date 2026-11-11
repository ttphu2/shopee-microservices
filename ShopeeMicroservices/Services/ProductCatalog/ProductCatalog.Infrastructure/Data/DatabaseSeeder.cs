using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ProductCatalog.Domain.Entities;
using ProductCatalog.Infrastructure.Settings;

namespace ProductCatalog.Infrastructure.Data
{
    // Inserts CatalogSeedData into each collection that is still empty, so restarts never duplicate or overwrite data.
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(
            IMongoClient client,
            IOptions<DatabaseSettings> options,
            CancellationToken cancellationToken = default)
        {
            var settings = options.Value;
            var database = client.GetDatabase(settings.DatabaseName);

            var brands = database.GetCollection<ProductBrand>(settings.BrandCollectionName);
            var types = database.GetCollection<ProductType>(settings.TypeCollectionName);
            var products = database.GetCollection<ProductSpu>(settings.ProductSpuCollectionName);
            var skus = database.GetCollection<ProductSku>(settings.ProductSkusCollectionName);

            if (await IsEmptyAsync(brands, cancellationToken))
            {
                await brands.InsertManyAsync(CatalogSeedData.CreateBrands(), cancellationToken: cancellationToken);
            }

            if (await IsEmptyAsync(types, cancellationToken))
            {
                await types.InsertManyAsync(CatalogSeedData.CreateTypes(), cancellationToken: cancellationToken);
            }

            if (await IsEmptyAsync(products, cancellationToken))
            {
                var (seedProducts, seedSkus) = CatalogSeedData.CreateProducts();

                // Products and their SKUs are inserted together, so a failure never leaves products without SKUs.
                using var session = await client.StartSessionAsync(cancellationToken: cancellationToken);
                await session.WithTransactionAsync(
                    async (transaction, token) =>
                    {
                        await products.InsertManyAsync(transaction, seedProducts, cancellationToken: token);
                        await skus.InsertManyAsync(transaction, seedSkus, cancellationToken: token);
                        return true;
                    },
                    cancellationToken: cancellationToken);
            }
        }

        private static async Task<bool> IsEmptyAsync<T>(IMongoCollection<T> collection, CancellationToken cancellationToken) =>
            !await collection.Find(FilterDefinition<T>.Empty).AnyAsync(cancellationToken);
    }
}
