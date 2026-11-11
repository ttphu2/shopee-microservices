using System.Net.Mime;
using Infrastructure.Abstractions.Messaging;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Application.DTOs;
using ProductCatalog.Application.Errors;
using ProductCatalog.Application.Handlers.CreateProductCommandHandler;
using ProductCatalog.Application.Handlers.DeleteProductCommandHandler;
using ProductCatalog.Application.Handlers.GetProductByIdQueryHandler;
using ProductCatalog.Application.Handlers.GetProductQueryHandler;
using ProductCatalog.Application.Handlers.UpdateProductCommandHandler;

namespace ProductCatalog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.ProblemJson)]
    public class ProductController(ISender sender) : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType<GetProductsQueryResult>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
        public async Task<IActionResult> GetProducts(CancellationToken cancellationToken)
        {
            var result = await sender.Send(new GetProductsQuery(), cancellationToken);

            return result.IsSuccess ? Ok(result.Value) : Problem(result.Error);
        }

        [HttpGet("{id}")]
        [ProducesResponseType<ProductDetailsDto>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, MediaTypeNames.Application.ProblemJson)]
        public async Task<IActionResult> GetProductById([FromRoute] string id, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new GetProductByIdQuery(id), cancellationToken);

            return result.IsSuccess ? Ok(result.Value) : Problem(result.Error);
        }

        [HttpPost]
        [ProducesResponseType<CreateProductResult>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.ProblemJson)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict, MediaTypeNames.Application.ProblemJson)]
        public async Task<IActionResult> CreateProduct(
            [FromBody] CreateProductRequest createProductRequest,
            CancellationToken cancellationToken)
        {
            var command = createProductRequest.Adapt<CreateProductCommand>();
            var result = await sender.Send(command, cancellationToken);

            return result.IsSuccess ? Ok(result.Value) : Problem(result.Error);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.ProblemJson)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, MediaTypeNames.Application.ProblemJson)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict, MediaTypeNames.Application.ProblemJson)]
        public async Task<IActionResult> UpdateProduct(
            [FromRoute] string id,
            [FromBody] UpdateProductRequest updateProductRequest,
            CancellationToken cancellationToken)
        {
            var command = updateProductRequest.Adapt<UpdateProductCommand>() with { Id = id };
            var result = await sender.Send(command, cancellationToken);

            return result.IsSuccess ? NoContent() : Problem(result.Error);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, MediaTypeNames.Application.ProblemJson)]
        public async Task<IActionResult> DeleteProduct([FromRoute] string id, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new DeleteProductCommand(id), cancellationToken);

            return result.IsSuccess ? NoContent() : Problem(result.Error);
        }

        // Turns a failed Result's Error into ProblemDetails. Request validation errors are exceptions and are
        // handled by ExceptionHandlingMiddleware instead.
        private ObjectResult Problem(Error error)
        {
            var statusCode = error.Code switch
            {
                ProductErrors.SkuCodeConflictCode => StatusCodes.Status409Conflict,
                _ when error == Error.NotFound => StatusCodes.Status404NotFound,
                _ => StatusCodes.Status400BadRequest
            };

            return Problem(title: error.Code, detail: error.Name, statusCode: statusCode);
        }
    }
}
