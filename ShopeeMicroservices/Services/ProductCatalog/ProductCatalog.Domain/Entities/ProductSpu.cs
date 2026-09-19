namespace ProductCatalog.Domain.Entities
{
    public class ProductSpu : BaseEntity
    {
        public string ProductCode { get; set; } = null!;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string BrandId { get; set; } = null!;
        public string CategoryId { get; set; } = null!;
        public List<string> Images { get; set; } = [];
        public List<VariantOption> VariantOptions { get; set; } = [];

        public static ProductSpu Create(
            string productCode,
            string name,
            string description,
            string brandId,
            string categoryId,
            IEnumerable<string>? images = null,
            IEnumerable<VariantOption>? variantOptions = null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(productCode);
            ArgumentException.ThrowIfNullOrWhiteSpace(name);

            if (string.IsNullOrEmpty(brandId))
            {
                throw new ArgumentException("BrandId is required.", nameof(brandId));
            }

            if (string.IsNullOrEmpty(categoryId))
            {
                throw new ArgumentException("CategoryId is required.", nameof(categoryId));
            }

            return new ProductSpu
            {
                ProductCode = productCode,
                Name = name,
                Description = description ?? string.Empty,
                BrandId = brandId,
                CategoryId = categoryId,
                Images = images?.ToList() ?? [],
                VariantOptions = variantOptions?.ToList() ?? []
            };
        }
    }

    public class VariantOption
    {
        public string Name { get; set; } = null!; // "Color", "Storage"
        public string Code { get; set; } = null!; // "color", "storage"
        public List<string> Values { get; set; } = [];   // ["Titanium Blue", "Black"], ["128GB", "256GB"]
    }
}
