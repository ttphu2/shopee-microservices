using Microsoft.Extensions.DependencyInjection;

namespace Basket.Infrastructure.Data
{
    // Creates the database and its table when they do not exist; an existing database is left unchanged.
    // No EF migrations: a model change needs the table recreated.
    public static class DatabaseInitializer
    {
        public static async Task InitializeAsync(IServiceProvider services, CancellationToken cancellationToken = default)
        {
            await using var scope = services.CreateAsyncScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<BasketDbContext>();

            await dbContext.Database.EnsureCreatedAsync(cancellationToken);
        }
    }
}
