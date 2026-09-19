using MongoDB.Bson.Serialization.Attributes;

namespace ProductCatalog.Domain.Entities
{
    public class ProductBrand : BaseEntity
    {
        [BsonElement("Name")]
        public string Name { get; set; } = null!;
    }
}