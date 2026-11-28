using Discount.Application.Errors;
using Discount.Domain.Repositories;
using Infrastructure.Abstractions.Messaging;
using Infrastructure.Errors;

namespace Discount.Application.Handlers.DeleteDiscountCommandHandler
{
    public class DeleteDiscountCommandHandler(ICouponRepository couponRepository)
        : ICommandHandler<DeleteDiscountCommand>
    {
        public async Task<ValidationOutcome> Handle(DeleteDiscountCommand request, CancellationToken cancellationToken)
        {
            var deleted = await couponRepository.DeleteAsync(request.Key, cancellationToken);

            if (!deleted)
            {
                return DiscountErrors.CouponNotFound;
            }

            return ValidationOutcome.Success();
        }
    }
}
