using Discount.Application.DTOs;
using Discount.Domain.Entities;

namespace Discount.Application.Mappers
{
    public static class CouponMapper
    {
        public static CouponDto ToDto(this Coupon entity)
        {
            return new CouponDto(entity.Id, entity.ProductId, entity.SkuCode, entity.Description, entity.Amount);
        }
    }
}
