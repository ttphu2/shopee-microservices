using Discount.Domain.Entities;
using Discount.Domain.ValueObjects;

namespace Discount.Domain.Repositories;

public interface ICouponRepository
{
    Task<Coupon?> GetAsync(CouponKey key, CancellationToken cancellationToken = default);

    Task<Coupon> AddAsync(Coupon coupon, CancellationToken cancellationToken = default);

    // Returns false when no coupon has coupon.Id.
    Task<bool> UpdateAsync(Coupon coupon, CancellationToken cancellationToken = default);

    // Returns false when no coupon matches the key.
    Task<bool> DeleteAsync(CouponKey key, CancellationToken cancellationToken = default);
}
