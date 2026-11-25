using Discount.Application.DTOs;
using Infrastructure.Abstractions.Messaging;

namespace Discount.Application.Handlers.CreateDiscountCommandHandler
{
    public record CreateDiscountCommand(string ProductId, string SkuCode, string Description, decimal Amount) : ICommand<CouponDto>;
}
