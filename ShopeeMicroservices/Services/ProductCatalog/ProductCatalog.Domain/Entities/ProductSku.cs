using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ProductCatalog.Domain.Entities
{
    public class ProductSku : BaseEntity
    {
        public string SpuId { get; set; } = null!;

        public string SkuCode { get; set; } = string.Empty;

        [BsonRepresentation(BsonType.Decimal128)]
        public decimal Price { get; set; }

        public string? ImageUrl { get; set; }

        public List<SkuAttribute> Attributes { get; set; } = [];


        public static ProductSku Create(
            string spuId,
            string skuCode,
            decimal price,
            string? imageUrl,
            IEnumerable<SkuAttribute> attributes)
        {
            var sku = new ProductSku
            {
                SpuId = spuId,
                SkuCode = skuCode,
                Price = price,
                ImageUrl = imageUrl
            };

            sku.Attributes.AddRange(attributes);
            return sku;
        }

        // SpuId and SkuCode identify the SKU and are not changed by an update.
        public void Update(
            decimal price,
            string? imageUrl,
            IEnumerable<SkuAttribute> attributes,
            DateTime modifiedAt)
        {
            Price = price;
            ImageUrl = imageUrl;
            Attributes = attributes.ToList();
            LastModified = modifiedAt;
        }
    }

    public class SkuAttribute
    {
        public string AttributeCode { get; set; } = string.Empty;   // "Color"

        public string Value { get; set; } = string.Empty; // "Titanium Blue"
    }
}
