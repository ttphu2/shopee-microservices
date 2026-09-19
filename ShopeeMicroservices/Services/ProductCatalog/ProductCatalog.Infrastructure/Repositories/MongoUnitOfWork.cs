using MongoDB.Driver;
using ProductCatalog.Domain.Abstractions;

namespace ProductCatalog.Infrastructure.Repositories
{
    public class MongoUnitOfWork : IUnitOfWork
    {
        private readonly IMongoClient _mongoClient;

        public MongoUnitOfWork(IMongoClient mongoClient)
        {
            _mongoClient = mongoClient;
        }

        public async Task<IClientSessionHandle> BeginTransactionAsync(
            CancellationToken cancellationToken = default)
        {
            var session = await _mongoClient.StartSessionAsync(
                cancellationToken: cancellationToken);

            session.StartTransaction();

            return session;
        }

        public async Task CommitAsync(
            IClientSessionHandle session,
            CancellationToken cancellationToken = default)
        {
            await session.CommitTransactionAsync(cancellationToken);
            session.Dispose();
        }

        public async Task AbortAsync(
            IClientSessionHandle session,
            CancellationToken cancellationToken = default)
        {
            if (session.IsInTransaction)
            {
                await session.AbortTransactionAsync(cancellationToken);
            }

            session.Dispose();
        }
    }
}
