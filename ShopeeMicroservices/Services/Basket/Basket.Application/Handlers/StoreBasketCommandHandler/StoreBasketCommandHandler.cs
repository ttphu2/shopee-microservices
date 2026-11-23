using Basket.Application.DTOs;
using Basket.Application.Mappers;
using Basket.Domain.Entities;
using Basket.Domain.Repositories;
using Infrastructure.Abstractions.Messaging;
using Infrastructure.Errors;

namespace Basket.Application.Handlers.StoreBasketCommandHandler
{
    public class StoreBasketCommandHandler(IBasketRepository basketRepository)
        : ICommandHandler<StoreBasketCommand, ShoppingCartDto>
    {
        public async Task<Validation<ShoppingCartDto>> Handle(StoreBasketCommand request, CancellationToken cancellationToken)
        {
            var basket = new ShoppingCart
            {
                UserName = request.UserName,
                Items = request.Items.Select(item => item.ToEntity()).ToList()
            };

            var storedBasket = await basketRepository.StoreBasketAsync(basket, cancellationToken);

            return storedBasket.ToDto();
        }
    }
}
