using Basket.Domain.Repositories;
using Infrastructure.Abstractions.Messaging;
using Infrastructure.Errors;

namespace Basket.Application.Handlers.DeleteBasketCommandHandler
{
    public class DeleteBasketCommandHandler(IBasketRepository basketRepository)
        : ICommandHandler<DeleteBasketCommand>
    {
        public async Task<ValidationOutcome> Handle(DeleteBasketCommand request, CancellationToken cancellationToken)
        {
            var deleted = await basketRepository.DeleteBasketAsync(request.UserName, cancellationToken);

            if (!deleted)
            {
                return Error.NotFound;
            }

            return ValidationOutcome.Success();
        }
    }
}
