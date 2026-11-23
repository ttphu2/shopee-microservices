using Basket.Application.DTOs;
using Infrastructure.Abstractions.Messaging;

namespace Basket.Application.Handlers.GetBasketQueryHandler
{
    // Queries are not run through ValidationBehavior (commands only); the route guarantees a non-empty user name.
    public record GetBasketQuery(string UserName) : IQuery<ShoppingCartDto>;
}
