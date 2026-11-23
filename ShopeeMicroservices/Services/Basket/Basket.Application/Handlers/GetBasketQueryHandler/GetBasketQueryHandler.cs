using Basket.Application.DTOs;
using Basket.Application.Mappers;
using Basket.Domain.Repositories;
using Infrastructure.Abstractions.Messaging;
using Infrastructure.Errors;

namespace Basket.Application.Handlers.GetBasketQueryHandler
{
    public class GetBasketQueryHandler(IBasketRepository basketRepository)
        : IQueryHandler<GetBasketQuery, ShoppingCartDto>
    {
        public async Task<Validation<ShoppingCartDto>> Handle(GetBasketQuery request, CancellationToken cancellationToken)
        {
            var basket = await basketRepository.GetBasketAsync(request.UserName, cancellationToken);

            if (basket is null)
            {
                return Error.NotFound;
            }

            return basket.ToDto();
        }
    }
}
