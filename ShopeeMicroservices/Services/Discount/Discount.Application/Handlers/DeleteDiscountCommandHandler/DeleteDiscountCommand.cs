using Discount.Domain.ValueObjects;
using Infrastructure.Abstractions.Messaging;

namespace Discount.Application.Handlers.DeleteDiscountCommandHandler
{
    public record DeleteDiscountCommand(CouponKey Key) : ICommand;
}
