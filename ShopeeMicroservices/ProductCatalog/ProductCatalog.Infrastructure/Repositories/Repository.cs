using MongoDB.Driver;
using ProductCatalog.Domain.Abstractions;

namespace ProductCatalog.Infrastructure.Repositories;

internal abstract class Repository<T>
    where T : Entity
{
    protected readonly IMongoCollection<T> Collection;

    protected Repository(IMongoDatabase database, string collectionName)
    {
        Collection = database.GetCollection<T>(collectionName);
    }

    public virtual async Task<T?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await Collection
            .Find(x => x.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public virtual Task AddAsync(
        T entity,
        CancellationToken cancellationToken = default)
    {
        return Collection.InsertOneAsync(entity, cancellationToken: cancellationToken);
    }
}