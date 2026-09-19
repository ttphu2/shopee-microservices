using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ProductCatalog.Domain.Entities;
using ProductCatalog.Infrastructure.Settings;
using System.Text.Json;

namespace ProductCatalog.Infrastructure.Data
{
    public static class DatabaseSeeder
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public static async Task SeedAsync(
            IOptions<DatabaseSettings> options)
        {
            var settings = options.Value;

            var client = new MongoClient(
                settings.ConnectionString);

            var db = client.GetDatabase(
                settings.DatabaseName);

            var brands = db.GetCollection<ProductBrand>(
                settings.BrandCollectionName);

            var types = db.GetCollection<ProductType>(
                settings.TypeCollectionName);

            var products = db.GetCollection<ProductSpu>(
                settings.ProductSpuCollectionName);

            var skus = db.GetCollection<ProductSku>(
                settings.ProductSkusCollectionName);

            var assemblyPath = Path.GetDirectoryName(typeof(DatabaseSeeder).Assembly.Location)!;
            var seedBasePath = Path.Combine(assemblyPath, "Data", "SeedData");

            await SeedBrandsAsync(
                brands,
                seedBasePath);

            await SeedTypesAsync(
                types,
                seedBasePath);

            await SeedProductsAsync(
                products,
                skus,
                brands,
                types,
                seedBasePath);
        }

        private static async Task SeedBrandsAsync(
            IMongoCollection<ProductBrand> brands,
            string seedBasePath)
        {
            if (await brands.CountDocumentsAsync(_ => true) > 0)
            {
                return;
            }

            var path = Path.Combine(
                seedBasePath,
                "brands.json");

            var data = await File.ReadAllTextAsync(path);

            var brandList =
                JsonSerializer.Deserialize<List<ProductBrand>>(
                    data,
                    JsonOptions) ?? [];

            var now = DateTime.UtcNow;

            foreach (var brand in brandList)
            {
                brand.Id = Guid.CreateVersion7().ToString();

                if (brand.CreatedDate == default)
                {
                    brand.CreatedDate = now;
                }
            }

            if (brandList.Count > 0)
            {
                await brands.InsertManyAsync(brandList);
            }
        }

        private static async Task SeedTypesAsync(
            IMongoCollection<ProductType> types,
            string seedBasePath)
        {
            if (await types.CountDocumentsAsync(_ => true) > 0)
            {
                return;
            }

            var path = Path.Combine(
                seedBasePath,
                "types.json");

            var data = await File.ReadAllTextAsync(path);

            var typeList =
                JsonSerializer.Deserialize<List<ProductType>>(
                    data,
                    JsonOptions) ?? [];

            var now = DateTime.UtcNow;

            foreach (var type in typeList)
            {
                type.Id = Guid.CreateVersion7().ToString();

                if (type.CreatedDate == default)
                {
                    type.CreatedDate = now;
                }
            }

            if (typeList.Count > 0)
            {
                await types.InsertManyAsync(typeList);
            }
        }

        private static async Task SeedProductsAsync(
            IMongoCollection<ProductSpu> products,
            IMongoCollection<ProductSku> skus,
            IMongoCollection<ProductBrand> brands,
            IMongoCollection<ProductType> types,
            string seedBasePath)
        {
            if (await products.CountDocumentsAsync(_ => true) > 0)
            {
                return;
            }

            var brand = await brands
                .Find(_ => true)
                .FirstOrDefaultAsync();

            var type = await types
                .Find(_ => true)
                .FirstOrDefaultAsync();

            var path = Path.Combine(
                seedBasePath,
                "products.json");

            var data = await File.ReadAllTextAsync(path);

            var seedList =
                JsonSerializer.Deserialize<List<ProductSeed>>(
                    data,
                    JsonOptions) ?? [];

            if (seedList.Count == 0)
            {
                return;
            }

            var now = DateTime.UtcNow;

            var productList = new List<ProductSpu>();
            var skuList = new List<ProductSku>();

            foreach (var item in seedList)
            {
                var spuId = Guid.CreateVersion7().ToString();

                var productSpu = new ProductSpu
                {
                    Id = spuId,
                    ProductCode = item.ProductCode,
                    Name = item.Name,
                    Description = item.Description,
                    BrandId = brand.Id,
                    CategoryId = type.Id,
                    Images = item.Images,
                    VariantOptions = item.VariantOptions,
                    CreatedDate = now
                };

                productList.Add(productSpu);

                foreach (var sku in item.Skus)
                {
                    sku.Id = Guid.CreateVersion7().ToString();
                    sku.SpuId = spuId;

                    if (sku.CreatedDate == default)
                    {
                        sku.CreatedDate = now;
                    }

                    skuList.Add(sku);
                }
            }

            await products.InsertManyAsync(productList);

            if (skuList.Count > 0)
            {
                await skus.InsertManyAsync(skuList);
            }
        }
    }
}
