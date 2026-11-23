namespace Basket.Domain.Entities;

// One basket per user. Stored as one PostgreSQL row (items in a jsonb column) and cached in Redis.
public class ShoppingCart
{
    public string UserName { get; set; } = default!;

    public List<ShoppingCartItem> Items { get; set; } = [];

    // Computed on read; not stored.
    public decimal TotalPrice => Items.Sum(item => item.Price * item.Quantity);
}
