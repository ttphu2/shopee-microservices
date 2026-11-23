using Basket.Application.DTOs;
using Basket.Application.Handlers.DeleteBasketCommandHandler;
using Basket.Application.Handlers.GetBasketQueryHandler;
using Basket.Application.Handlers.StoreBasketCommandHandler;
using Infrastructure.Errors;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace Basket.API.Controllers
{
    [Route("basket")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.ProblemJson)]
    public class BasketController(ISender sender) : ControllerBase
    {
        [HttpGet("{userName}")]
        public Task<Validation<ShoppingCartDto>> GetBasket([FromRoute] string userName, CancellationToken cancellationToken)
        {
            return sender.Send(new GetBasketQuery(userName), cancellationToken);
        }

        // PUT: the request replaces the user's whole basket, so repeating it gives the same result.
        [HttpPut("{userName}")]
        public Task<Validation<ShoppingCartDto>> StoreBasket(
            [FromRoute] string userName,
            [FromBody] StoreBasketRequest storeBasketRequest,
            CancellationToken cancellationToken)
        {
            var command = storeBasketRequest.Adapt<StoreBasketCommand>() with { UserName = userName };

            return sender.Send(command, cancellationToken);
        }

        [HttpDelete("{userName}")]
        public Task<ValidationOutcome> DeleteBasket([FromRoute] string userName, CancellationToken cancellationToken)
        {
            return sender.Send(new DeleteBasketCommand(userName), cancellationToken);
        }
    }
}
