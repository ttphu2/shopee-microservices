using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Domain.Products
{
    public sealed class ProductDetails
    {
        public ProductSpu Spu { get; init; }
        public List<ProductSku> Skus { get; init; } = [];
    }
}
