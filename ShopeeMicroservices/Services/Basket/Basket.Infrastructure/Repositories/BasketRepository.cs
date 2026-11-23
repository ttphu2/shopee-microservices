using Basket.Domain.Entities;
using Basket.Domain.Repositories;
using Basket.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Basket.Infrastructure.Repositories
{
    // PostgreSQL store. Handlers use it through CachedBasketRepository.
    public class BasketRepository(BasketDbContext dbContext) : IBasketRepository
    {
        public async Task<ShoppingCart?> GetBasketAsync(string userName, CancellationToken cancellationToken = default)
        {
            return await dbContext.ShoppingCarts
                .AsNoTracking()
                .SingleOrDefaultAsync(cart => cart.UserName == userName, cancellationToken);
        }

        public async Task<ShoppingCart> StoreBasketAsync(ShoppingCart basket, CancellationToken cancellationToken = default)
        {
            var existing = await dbContext.ShoppingCarts
                .SingleOrDefaultAsync(cart => cart.UserName == basket.UserName, cancellationToken);

            if (existing is null)
            {
                dbContext.ShoppingCarts.Add(basket);
            }
            else
            {
                existing.Items = basket.Items;
            }

            await dbContext.SaveChangesAsync(cancellationToken);

            return existing ?? basket;
        }

        public async Task<bool> DeleteBasketAsync(string userName, CancellationToken cancellationToken = default)
        {
            var deleted = await dbContext.ShoppingCarts
                .Where(cart => cart.UserName == userName)
                .ExecuteDeleteAsync(cancellationToken);

            return deleted > 0;
        }
    }
}
