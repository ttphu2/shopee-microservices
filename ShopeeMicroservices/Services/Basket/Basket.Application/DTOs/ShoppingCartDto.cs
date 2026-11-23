namespace Basket.Application.DTOs
{
    public sealed record ShoppingCartDto(
        string UserName,
        IReadOnlyList<ShoppingCartItemDto> Items,
        decimal TotalPrice
    );
}
