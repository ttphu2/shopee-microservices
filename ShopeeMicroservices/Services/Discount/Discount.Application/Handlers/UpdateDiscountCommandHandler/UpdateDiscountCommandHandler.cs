using Discount.Application.DTOs;
using Discount.Application.Errors;
using Discount.Application.Mappers;
using Discount.Domain.Entities;
using Discount.Domain.Repositories;
using Infrastructure.Abstractions.Messaging;
using Infrastructure.Errors;

namespace Discount.Application.Handlers.UpdateDiscountCommandHandler
{
    public class UpdateDiscountCommandHandler(ICouponRepository couponRepository)
        : ICommandHandler<UpdateDiscountCommand, CouponDto>
    {
        public async Task<Validation<CouponDto>> Handle(UpdateDiscountCommand request, CancellationToken cancellationToken)
        {
            var coupon = new Coupon
            {
                Id = request.Id,
                ProductId = request.ProductId,
                SkuCode = request.SkuCode,
                Description = request.Description,
                Amount = request.Amount
            };

            var updated = await couponRepository.UpdateAsync(coupon, cancellationToken);

            if (!updated)
            {
                return DiscountErrors.CouponNotFound;
            }

            return coupon.ToDto();
        }
    }
}
