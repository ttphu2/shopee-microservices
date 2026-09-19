using MongoDB.Bson.Serialization.Attributes;

namespace ProductCatalog.Domain.Abstractions;

public abstract class Entity<T> : IEntity<T>
{
    [BsonId]
    public T Id { get; set; } = default!;
    public DateTime? CreatedDate { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? LastModified { get; set; }
    public string? LastModifiedBy { get; set; }
}
