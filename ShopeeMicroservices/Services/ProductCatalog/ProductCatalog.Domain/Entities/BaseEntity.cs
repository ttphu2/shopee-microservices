using ProductCatalog.Domain.Abstractions;

namespace ProductCatalog.Domain.Entities
{
    public abstract class BaseEntity : Entity<string>
    {
        protected BaseEntity()
        {
            Id = Guid.CreateVersion7().ToString();
            CreatedDate = DateTime.UtcNow;
        }
    }
}
