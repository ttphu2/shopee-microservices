using Basket.Application.DTOs;
using Infrastructure.Abstractions.Messaging;

namespace Basket.Application.Handlers.StoreBasketCommandHandler
{
    // Replaces the user's whole basket (creates it when missing). An empty item list stores an empty basket.
    public record StoreBasketCommand(
        string UserName,
        List<ShoppingCartItemDto> Items
    ) : ICommand<ShoppingCartDto>;
}
