using System.Text.Json;
using Basket.Domain.Entities;
using Basket.Domain.Repositories;
using Microsoft.Extensions.Caching.Distributed;

namespace Basket.Infrastructure.Repositories
{
    // Cache-aside decorator over the PostgreSQL repository (Redis via IDistributedCache):
    // - read: return the cached basket; on a miss, load it from the database and cache it.
    // - write/delete: change the database first, then remove the cached copy, so the next read reloads it.
    //   Removing (instead of overwriting) means a cache entry is only ever built from what the database returned.
    public class CachedBasketRepository(IBasketRepository repository, IDistributedCache cache) : IBasketRepository
    {
        // Idle baskets drop out of Redis; the database stays the source of truth.
        private static readonly DistributedCacheEntryOptions CacheEntryOptions = new()
        {
            SlidingExpiration = TimeSpan.FromMinutes(30)
        };

        public async Task<ShoppingCart?> GetBasketAsync(string userName, CancellationToken cancellationToken = default)
        {
            var cachedBasket = await cache.GetStringAsync(userName, cancellationToken);
            if (cachedBasket is not null)
            {
                return JsonSerializer.Deserialize<ShoppingCart>(cachedBasket);
            }

            var basket = await repository.GetBasketAsync(userName, cancellationToken);
            if (basket is not null)
            {
                await cache.SetStringAsync(userName, JsonSerializer.Serialize(basket), CacheEntryOptions, cancellationToken);
            }

            return basket;
        }

        public async Task<ShoppingCart> StoreBasketAsync(ShoppingCart basket, CancellationToken cancellationToken = default)
        {
            var storedBasket = await repository.StoreBasketAsync(basket, cancellationToken);
            await cache.RemoveAsync(basket.UserName, cancellationToken);

            return storedBasket;
        }

        public async Task<bool> DeleteBasketAsync(string userName, CancellationToken cancellationToken = default)
        {
            var deleted = await repository.DeleteBasketAsync(userName, cancellationToken);
            await cache.RemoveAsync(userName, cancellationToken);

            return deleted;
        }
    }
}
