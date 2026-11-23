using Basket.Application.DTOs;
using Basket.Domain.Entities;

namespace Basket.Application.Mappers
{
    public static class BasketMapper
    {
        public static ShoppingCartDto ToDto(this ShoppingCart entity)
        {
            return new ShoppingCartDto(
                entity.UserName,
                entity.Items.Select(ToDto).ToList(),
                entity.TotalPrice);
        }

        public static ShoppingCartItem ToEntity(this ShoppingCartItemDto dto)
        {
            return new ShoppingCartItem
            {
                ProductId = dto.ProductId,
                SkuCode = dto.SkuCode,
                ProductName = dto.ProductName,
                Price = dto.Price,
                Quantity = dto.Quantity
            };
        }

        private static ShoppingCartItemDto ToDto(ShoppingCartItem entity)
        {
            return new ShoppingCartItemDto(
                entity.ProductId,
                entity.SkuCode,
                entity.ProductName,
                entity.Price,
                entity.Quantity);
        }
    }
}
