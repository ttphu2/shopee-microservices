namespace Basket.Application.DTOs
{
    // Body of PUT /basket/{userName}; the user name comes from the route.
    public record StoreBasketRequest(List<ShoppingCartItemDto> Items);
}
