using Discount.Application.DTOs;
using Discount.Application.Mappers;
using Discount.Domain.Repositories;
using Infrastructure.Abstractions.Messaging;
using Infrastructure.Errors;

namespace Discount.Application.Handlers.GetDiscountQueryHandler
{
    public class GetDiscountQueryHandler(ICouponRepository couponRepository)
        : IQueryHandler<GetDiscountQuery, CouponDto>
    {
        public async Task<Validation<CouponDto>> Handle(GetDiscountQuery request, CancellationToken cancellationToken)
        {
            var coupon = await couponRepository.GetAsync(request.Key, cancellationToken);

            if (coupon is null)
            {
                return Error.NotFound;
            }

            return coupon.ToDto();
        }
    }
}
