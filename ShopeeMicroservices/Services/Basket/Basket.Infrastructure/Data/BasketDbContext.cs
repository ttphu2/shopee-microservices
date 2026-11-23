using Basket.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Basket.Infrastructure.Data;

public class BasketDbContext(DbContextOptions<BasketDbContext> options) : DbContext(options)
{
    public DbSet<ShoppingCart> ShoppingCarts => Set<ShoppingCart>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ShoppingCart>(cart =>
        {
            cart.HasKey(c => c.UserName);

            // The items are always read and replaced together with their basket, so they live in one jsonb column.
            cart.OwnsMany(c => c.Items, items => items.ToJson());
        });
    }
}
