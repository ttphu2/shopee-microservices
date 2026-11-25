namespace Discount.Application.DTOs
{
    public record CouponDto(int Id, string ProductId, string SkuCode, string Description, decimal Amount);
}
