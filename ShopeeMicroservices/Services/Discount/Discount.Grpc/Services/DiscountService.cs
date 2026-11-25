using Discount.Grpc.Mappers;
using Grpc.Core;
using MediatR;

namespace Discount.Grpc.Services
{
    public class DiscountService(ISender sender) : DiscountProtoService.DiscountProtoServiceBase
    {
        public override async Task<CouponResponse> GetDiscount(GetDiscountRequest request, ServerCallContext context)
        {
            var result = await sender.Send(request.ToQuery(), context.CancellationToken);

            return result.GetValueOrThrow().ToResponse();
        }

        public override async Task<CouponResponse> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
        {
            var result = await sender.Send(request.ToCommand(), context.CancellationToken);

            return result.GetValueOrThrow().ToResponse();
        }

        public override async Task<CouponResponse> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
        {
            var result = await sender.Send(request.ToCommand(), context.CancellationToken);

            return result.GetValueOrThrow().ToResponse();
        }

        public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
        {
            var result = await sender.Send(request.ToCommand(), context.CancellationToken);
            result.ThrowIfFailed();

            return new DeleteDiscountResponse { Success = true };
        }
    }
}
