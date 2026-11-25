using Discount.Domain.Entities;
using Discount.Domain.Repositories;
using Discount.Domain.ValueObjects;
using Discount.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Discount.Infrastructure.Repositories
{
    public class CouponRepository(DiscountDbContext dbContext) : ICouponRepository
    {
        // Key lookups are served by the unique SkuCode index (at most one row), so no (ProductId, SkuCode) index is needed.
        public async Task<Coupon?> GetAsync(CouponKey key, CancellationToken cancellationToken = default)
        {
            return await dbContext.Coupons
                .AsNoTracking()
                .SingleOrDefaultAsync(coupon => coupon.SkuCode == key.SkuCode && coupon.ProductId == key.ProductId, cancellationToken);
        }

        public async Task<Coupon> AddAsync(Coupon coupon, CancellationToken cancellationToken = default)
        {
            dbContext.Coupons.Add(coupon);

            await dbContext.SaveChangesAsync(cancellationToken);

            return coupon;
        }

        public async Task<bool> UpdateAsync(Coupon coupon, CancellationToken cancellationToken = default)
        {
            var updated = await dbContext.Coupons
                .Where(existing => existing.Id == coupon.Id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(existing => existing.ProductId, coupon.ProductId)
                    .SetProperty(existing => existing.SkuCode, coupon.SkuCode)
                    .SetProperty(existing => existing.Description, coupon.Description)
                    .SetProperty(existing => existing.Amount, coupon.Amount),
                    cancellationToken);

            return updated > 0;
        }

        public async Task<bool> DeleteAsync(CouponKey key, CancellationToken cancellationToken = default)
        {
            var deleted = await dbContext.Coupons
                .Where(coupon => coupon.SkuCode == key.SkuCode && coupon.ProductId == key.ProductId)
                .ExecuteDeleteAsync(cancellationToken);

            return deleted > 0;
        }
    }
}
