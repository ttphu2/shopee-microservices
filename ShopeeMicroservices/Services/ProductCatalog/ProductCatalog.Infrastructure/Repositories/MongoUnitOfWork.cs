using MongoDB.Driver;
using ProductCatalog.Domain.Abstractions;

namespace ProductCatalog.Infrastructure.Repositories
{
    public class MongoUnitOfWork : IUnitOfWork, IMongoSessionAccessor
    {
        private readonly IMongoClient _mongoClient;

        public MongoUnitOfWork(IMongoClient mongoClient)
        {
            _mongoClient = mongoClient;
        }

        public IClientSessionHandle? CurrentSession { get; private set; }

        public async Task<IClientSessionHandle> BeginTransactionAsync(
            CancellationToken cancellationToken = default)
        {
            var session = await _mongoClient.StartSessionAsync(
                cancellationToken: cancellationToken);

            session.StartTransaction();

            CurrentSession = session;

            return session;
        }

        public async Task CommitAsync(
            IClientSessionHandle session,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await session.CommitTransactionAsync(cancellationToken);
            }
            finally
            {
                CurrentSession = null;
                session.Dispose();
            }
        }

        public async Task AbortAsync(
            IClientSessionHandle session,
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (session.IsInTransaction)
                {
                    await session.AbortTransactionAsync(cancellationToken);
                }
            }
            finally
            {
                CurrentSession = null;
                session.Dispose();
            }
        }
    }
}
