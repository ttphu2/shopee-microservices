using Basket.Domain.Entities;

namespace Basket.Domain.Repositories;

public interface IBasketRepository
{
    Task<ShoppingCart?> GetBasketAsync(string userName, CancellationToken cancellationToken = default);

    // Creates the basket or replaces the existing one (all items); returns the stored basket.
    Task<ShoppingCart> StoreBasketAsync(ShoppingCart basket, CancellationToken cancellationToken = default);

    // Returns false when the user has no basket.
    Task<bool> DeleteBasketAsync(string userName, CancellationToken cancellationToken = default);
}
