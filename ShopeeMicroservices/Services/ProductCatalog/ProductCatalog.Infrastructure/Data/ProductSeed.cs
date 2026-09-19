using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Infrastructure.Data
{
    public class ProductSeed
    {
        public string ProductCode { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string BrandId { get; set; } = null!;
        public string CategoryId { get; set; } = null!;
        public List<string> Images { get; set; } = [];
        public List<VariantOption> VariantOptions { get; set; } = [];
        public List<ProductSku> Skus { get; set; } = [];
    }
}
