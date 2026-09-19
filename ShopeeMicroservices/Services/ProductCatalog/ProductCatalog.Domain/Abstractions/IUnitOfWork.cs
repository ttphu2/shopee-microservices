using MongoDB.Driver;

namespace ProductCatalog.Domain.Abstractions;

public interface IUnitOfWork
{
    Task<IClientSessionHandle> BeginTransactionAsync(CancellationToken cancellationToken = default);

    Task CommitAsync(IClientSessionHandle session, CancellationToken cancellationToken = default);

    Task AbortAsync(IClientSessionHandle session, CancellationToken cancellationToken = default);
}