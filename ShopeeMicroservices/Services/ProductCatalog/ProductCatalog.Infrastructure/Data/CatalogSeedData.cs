using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Infrastructure.Data
{
    // Sample catalog inserted by DatabaseSeeder. Brand, type and product ids are fixed so they are the same in every
    // fresh database and can be used in ProductCatalog.API/http/catalog.http.
    public static class CatalogSeedData
    {
        public const string AppleBrandId = "019a7e00-0000-7000-8000-000000000101";
        public const string SamsungBrandId = "019a7e00-0000-7000-8000-000000000102";
        public const string GoogleBrandId = "019a7e00-0000-7000-8000-000000000103";

        public const string SmartphoneTypeId = "019a7e00-0000-7000-8000-000000000201";
        public const string TabletTypeId = "019a7e00-0000-7000-8000-000000000202";
        public const string LaptopTypeId = "019a7e00-0000-7000-8000-000000000203";

        // Every sample product is an Apple smartphone sold in two colors x two storage sizes; the price depends only
        // on the storage size.
        private static readonly PhoneSeed[] Phones =
        [
            new("019a7e00-0000-7000-8000-000000000301", "IPHONE-X", "iPhone X",
                [new("Space Gray", "SG"), new("Silver", "SL")], [new("64GB", 799m), new("256GB", 899m)]),
            new("019a7e00-0000-7000-8000-000000000302", "IPHONE-11", "iPhone 11",
                [new("Black", "BK"), new("White", "WH")], [new("64GB", 699m), new("128GB", 749m)]),
            new("019a7e00-0000-7000-8000-000000000303", "IPHONE-12", "iPhone 12",
                [new("Black", "BK"), new("Blue", "BL")], [new("64GB", 799m), new("128GB", 849m)]),
            new("019a7e00-0000-7000-8000-000000000304", "IPHONE-13", "iPhone 13",
                [new("Midnight", "MN"), new("Blue", "BL")], [new("128GB", 799m), new("256GB", 899m)]),
            new("019a7e00-0000-7000-8000-000000000305", "IPHONE-14", "iPhone 14",
                [new("Midnight", "MN"), new("Blue", "BL")], [new("128GB", 899m), new("256GB", 999m)]),
            new("019a7e00-0000-7000-8000-000000000306", "IPHONE-15", "iPhone 15",
                [new("Black", "BK"), new("Blue", "BL")], [new("128GB", 899m), new("256GB", 999m)]),
            new("019a7e00-0000-7000-8000-000000000307", "IPHONE-16", "iPhone 16",
                [new("Black", "BK"), new("Ultramarine", "UB")], [new("128GB", 999m), new("256GB", 1099m)]),
            new("019a7e00-0000-7000-8000-000000000308", "IPHONE-17", "iPhone 17",
                [new("Black", "BK"), new("Titanium Blue", "TB")], [new("256GB", 1099m), new("512GB", 1299m)]),
        ];

        public static List<ProductBrand> CreateBrands() =>
        [
            new() { Id = AppleBrandId, Name = "Apple" },
            new() { Id = SamsungBrandId, Name = "Samsung" },
            new() { Id = GoogleBrandId, Name = "Google" },
        ];

        public static List<ProductType> CreateTypes() =>
        [
            new() { Id = SmartphoneTypeId, Name = "Smartphone" },
            new() { Id = TabletTypeId, Name = "Tablet" },
            new() { Id = LaptopTypeId, Name = "Laptop" },
        ];

        public static (List<ProductSpu> Products, List<ProductSku> Skus) CreateProducts()
        {
            var products = new List<ProductSpu>();
            var skus = new List<ProductSku>();

            foreach (var phone in Phones)
            {
                var imageBaseUrl = $"https://example.com/products/{phone.ProductCode.ToLowerInvariant()}";

                var product = ProductSpu.Create(
                    phone.ProductCode,
                    phone.Name,
                    $"Apple {phone.Name}",
                    AppleBrandId,
                    SmartphoneTypeId,
                    [$"{imageBaseUrl}/front.jpg", $"{imageBaseUrl}/back.jpg"],
                    [
                        new VariantOption { Name = "Color", Code = "color", Values = phone.Colors.Select(c => c.Name).ToList() },
                        new VariantOption { Name = "Storage", Code = "storage", Values = phone.Storages.Select(s => s.Size).ToList() },
                    ]);
                product.Id = phone.Id;
                products.Add(product);

                // e.g. SKU "IPHONE-X-SG-64" with image ".../iphone-x/space-gray.jpg"
                foreach (var color in phone.Colors)
                {
                    foreach (var storage in phone.Storages)
                    {
                        skus.Add(ProductSku.Create(
                            product.Id,
                            $"{phone.ProductCode}-{color.Code}-{storage.Size.Replace("GB", string.Empty)}",
                            storage.Price,
                            $"{imageBaseUrl}/{color.Name.ToLowerInvariant().Replace(' ', '-')}.jpg",
                            [
                                new SkuAttribute { AttributeCode = "color", Value = color.Name },
                                new SkuAttribute { AttributeCode = "storage", Value = storage.Size },
                            ]));
                    }
                }
            }

            return (products, skus);
        }

        private sealed record PhoneSeed(string Id, string ProductCode, string Name, ColorSeed[] Colors, StorageSeed[] Storages);

        private sealed record ColorSeed(string Name, string Code);

        private sealed record StorageSeed(string Size, decimal Price);
    }
}
