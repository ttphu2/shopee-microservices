using Basket.Application.Abstractions.Discount;
using Basket.Application.DTOs;
using Basket.Application.Mappers;
using Basket.Domain.Entities;
using Basket.Domain.Repositories;
using Infrastructure.Abstractions.Messaging;
using Infrastructure.Errors;

namespace Basket.Application.Handlers.StoreBasketCommandHandler
{
    public class StoreBasketCommandHandler(IBasketRepository basketRepository, IDiscountService discountService)
        : ICommandHandler<StoreBasketCommand, ShoppingCartDto>
    {
        public async Task<Validation<ShoppingCartDto>> Handle(StoreBasketCommand request, CancellationToken cancellationToken)
        {
            var basket = new ShoppingCart
            {
                UserName = request.UserName,
                Items = request.Items.Select(item => item.ToEntity()).ToList()
            };

            await ApplyDiscountsAsync(basket, cancellationToken);

            var storedBasket = await basketRepository.StoreBasketAsync(basket, cancellationToken);

            return storedBasket.ToDto();
        }

        private async Task ApplyDiscountsAsync(ShoppingCart basket, CancellationToken cancellationToken)
        {
            foreach (var item in basket.Items)
            {
                var amount = await discountService.GetDiscountAmountAsync(item.ProductId, item.SkuCode, cancellationToken);
                item.ApplyDiscount(amount);
            }
        }
    }
}
