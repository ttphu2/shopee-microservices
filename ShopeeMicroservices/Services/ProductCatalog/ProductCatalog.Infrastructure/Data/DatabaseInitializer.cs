using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using ProductCatalog.Infrastructure.Settings;

namespace ProductCatalog.Infrastructure.Data
{
    // Creates the sample data and the indexes, but only when the database does not exist yet.
    // An existing database is never touched, so later index or seed changes need a new (or dropped) database.
    public static class DatabaseInitializer
    {
        public static async Task InitializeAsync(
            IMongoClient client,
            IOptions<DatabaseSettings> options,
            ILogger logger,
            CancellationToken cancellationToken = default)
        {
            var databaseName = options.Value.DatabaseName;

            if (await DatabaseExistsAsync(client, databaseName, cancellationToken))
            {
                logger.LogInformation("Database {DatabaseName} exists; skipping index creation and seeding", databaseName);
                return;
            }

            logger.LogInformation("Database {DatabaseName} not found; seeding sample data and creating indexes", databaseName);

            // Seeding runs first and in one transaction: if it fails, nothing is written, the database still does not
            // exist and the next startup retries. If index creation fails afterwards, drop the database to rerun both.
            await DatabaseSeeder.SeedAsync(client, options, cancellationToken);
            await DatabaseIndexes.EnsureAsync(client, options, cancellationToken);
        }

        // MongoDB creates a database on its first write, so it "exists" once any collection does.
        private static async Task<bool> DatabaseExistsAsync(
            IMongoClient client,
            string databaseName,
            CancellationToken cancellationToken)
        {
            var listOptions = new ListDatabaseNamesOptions
            {
                Filter = Builders<BsonDocument>.Filter.Eq("name", databaseName)
            };

            using var names = await client.ListDatabaseNamesAsync(listOptions, cancellationToken);
            return await names.AnyAsync(cancellationToken);
        }
    }
}
