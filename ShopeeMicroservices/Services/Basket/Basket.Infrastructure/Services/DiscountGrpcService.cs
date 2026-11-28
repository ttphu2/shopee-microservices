using Basket.Application.Abstractions.Discount;
using Discount.Grpc;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Basket.Infrastructure.Services
{
    public class DiscountGrpcService(
        DiscountProtoService.DiscountProtoServiceClient client,
        ILogger<DiscountGrpcService> logger) : IDiscountService
    {
        private const decimal NanoFactor = 1_000_000_000m;

        private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(5);

        public async Task<decimal> GetDiscountAmountAsync(string productId, string skuCode, CancellationToken cancellationToken = default)
        {
            try
            {
                var coupon = await client.GetDiscountAsync(
                    new GetDiscountRequest { ProductId = productId, SkuCode = skuCode },
                    deadline: DateTime.UtcNow.Add(Timeout),
                    cancellationToken: cancellationToken);

                return ToDecimal(coupon.Amount);
            }
            catch (RpcException exception) when (exception.StatusCode == StatusCode.NotFound)
            {
                return 0m;
            }
            catch (RpcException exception) when (exception.StatusCode != StatusCode.Cancelled)
            {
                logger.LogWarning(
                    exception,
                    "Discount service failed for {ProductId}/{SkuCode} ({StatusCode}); no discount applied",
                    productId,
                    skuCode,
                    exception.StatusCode);

                return 0m;
            }
        }

        private static decimal ToDecimal(DecimalValue? value)
        {
            return value is null ? 0m : value.Units + value.Nanos / NanoFactor;
        }
    }
}
