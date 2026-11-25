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
                return Error.NotFound;
            }

            return ValidationOutcome.Success();
        }
    }
}
