using System.Globalization;
using Discount.Application.DTOs;
using Discount.Application.Mappers;
using Discount.Application.Resources;
using Discount.Domain.Entities;
using Discount.Domain.Repositories;
using Discount.Domain.ValueObjects;
using Infrastructure.Abstractions.Messaging;
using Infrastructure.Errors;

namespace Discount.Application.Handlers.CreateDiscountCommandHandler
{
    public class CreateDiscountCommandHandler(ICouponRepository couponRepository)
        : ICommandHandler<CreateDiscountCommand, CouponDto>
    {
        public async Task<Validation<CouponDto>> Handle(CreateDiscountCommand request, CancellationToken cancellationToken)
        {
            // The same SKU under another product id is not caught here; the unique SkuCode index rejects it (Unknown).
            var existing = await couponRepository.GetAsync(new CouponKey(request.ProductId, request.SkuCode), cancellationToken);

            if (existing is not null)
            {
                return Error.AlreadyExists with
                {
                    Message = string.Format(CultureInfo.CurrentCulture, ServerMessages.CouponAlreadyExists, request.SkuCode)
                };
            }

            var coupon = await couponRepository.AddAsync(
                new Coupon
                {
                    ProductId = request.ProductId,
                    SkuCode = request.SkuCode,
                    Description = request.Description,
                    Amount = request.Amount
                },
                cancellationToken);

            return coupon.ToDto();
        }
    }
}
