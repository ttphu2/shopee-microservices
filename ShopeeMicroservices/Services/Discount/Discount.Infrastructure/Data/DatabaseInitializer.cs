using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Discount.Infrastructure.Data
{
    // Applies pending EF Core migrations (Migrations folder), creating the database on first run.
    // Seed coupons are added only when the database is new, so deleted coupons do not come back on restart.
    public static class DatabaseInitializer
    {
        public static async Task InitializeAsync(IServiceProvider services, CancellationToken cancellationToken = default)
        {
            await using var scope = services.CreateAsyncScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<DiscountDbContext>();

            // On a new database EF logs one failed query here (the migrations history table does not exist yet);
            // that is expected and EF then treats the list as empty.
            var isNewDatabase = !(await dbContext.Database.GetAppliedMigrationsAsync(cancellationToken)).Any();

            await dbContext.Database.MigrateAsync(cancellationToken);

            if (isNewDatabase)
            {
                dbContext.Coupons.AddRange(DiscountSeedData.CreateCoupons());
                await dbContext.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
