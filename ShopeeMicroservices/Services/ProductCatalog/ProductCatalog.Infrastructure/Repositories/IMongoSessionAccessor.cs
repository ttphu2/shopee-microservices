using MongoDB.Driver;

namespace ProductCatalog.Infrastructure.Repositories
{
    // Exposes the transaction session started by the unit of work to repositories in the same request scope.
    public interface IMongoSessionAccessor
    {
        IClientSessionHandle? CurrentSession { get; }
    }
}
