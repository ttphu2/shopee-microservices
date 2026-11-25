using Discount.Application.DTOs;
using Infrastructure.Abstractions.Messaging;

namespace Discount.Application.Handlers.UpdateDiscountCommandHandler
{
    public record UpdateDiscountCommand(int Id, string ProductId, string SkuCode, string Description, decimal Amount) : ICommand<CouponDto>;
}
