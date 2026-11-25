using Discount.Application.DTOs;
using Discount.Domain.ValueObjects;
using Infrastructure.Abstractions.Messaging;

namespace Discount.Application.Handlers.GetDiscountQueryHandler
{
    public record GetDiscountQuery(CouponKey Key) : IQuery<CouponDto>;
}
