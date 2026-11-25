using Discount.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Discount.Infrastructure.Data.Configurations
{
    public class CouponConfiguration : IEntityTypeConfiguration<Coupon>
    {
        public void Configure(EntityTypeBuilder<Coupon> builder)
        {
            builder.HasKey(coupon => coupon.Id);

            builder.Property(coupon => coupon.ProductId).IsRequired();

            builder.Property(coupon => coupon.SkuCode).IsRequired();
            builder.HasIndex(coupon => coupon.SkuCode).IsUnique();

            builder.Property(coupon => coupon.Description).IsRequired();

            builder.Property(coupon => coupon.Amount).HasPrecision(18, 2);
        }
    }
}
