using Discount.Application.DTOs;
using Discount.Application.Handlers.CreateDiscountCommandHandler;
using Discount.Application.Handlers.DeleteDiscountCommandHandler;
using Discount.Application.Handlers.GetDiscountQueryHandler;
using Discount.Application.Handlers.UpdateDiscountCommandHandler;
using Discount.Domain.ValueObjects;

namespace Discount.Grpc.Mappers
{
    // Converts between the generated protobuf messages and the application's commands, queries and DTOs.
    public static class DiscountProtoMapper
    {
        private const decimal NanoFactor = 1_000_000_000m;

        public static GetDiscountQuery ToQuery(this GetDiscountRequest request)
        {
            return new GetDiscountQuery(new CouponKey(request.ProductId, request.SkuCode));
        }

        public static CreateDiscountCommand ToCommand(this CreateDiscountRequest request)
        {
            return new CreateDiscountCommand(request.ProductId, request.SkuCode, request.Description, request.Amount.ToDecimal());
        }

        public static UpdateDiscountCommand ToCommand(this UpdateDiscountRequest request)
        {
            return new UpdateDiscountCommand(request.Id, request.ProductId, request.SkuCode, request.Description, request.Amount.ToDecimal());
        }

        public static DeleteDiscountCommand ToCommand(this DeleteDiscountRequest request)
        {
            return new DeleteDiscountCommand(new CouponKey(request.ProductId, request.SkuCode));
        }

        public static CouponResponse ToResponse(this CouponDto dto)
        {
            return new CouponResponse
            {
                Id = dto.Id,
                ProductId = dto.ProductId,
                SkuCode = dto.SkuCode,
                Description = dto.Description,
                Amount = dto.Amount.ToDecimalValue()
            };
        }

        // A missing amount becomes 0, which the validators reject.
        private static decimal ToDecimal(this DecimalValue? value)
        {
            return value is null ? 0m : value.Units + value.Nanos / NanoFactor;
        }

        private static DecimalValue ToDecimalValue(this decimal value)
        {
            var units = decimal.ToInt64(decimal.Truncate(value));

            return new DecimalValue
            {
                Units = units,
                Nanos = decimal.ToInt32((value - units) * NanoFactor)
            };
        }
    }
}
