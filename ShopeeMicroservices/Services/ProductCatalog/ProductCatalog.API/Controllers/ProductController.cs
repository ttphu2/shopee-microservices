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
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IMediator mediator, ILogger<ProductController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(
            [FromBody] CreateProductRequest createProductRequest,
            CancellationToken cancellationToken)
        {
            var command = createProductRequest.Adapt<CreateProductCommand>();
            var result = await _mediator.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                return CommandFailure(result.Error);
            }

            return Ok(result.Value);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(
            [FromRoute] string id,
            [FromBody] UpdateProductRequest updateProductRequest,
            CancellationToken cancellationToken)
        {
            var command = updateProductRequest.Adapt<UpdateProductCommand>() with { Id = id };
            var result = await _mediator.Send(command, cancellationToken);

            return result.IsFailure ? CommandFailure(result.Error) : NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(
            [FromRoute] string id,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new DeleteProductCommand(id), cancellationToken);

            return result.IsFailure ? CommandFailure(result.Error) : NoContent();
        }

        [HttpGet]
        public async Task<ActionResult<GetProductsQueryResult>> GetProducts()
        {
            var query = new GetProductsQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDetailsDto>> GetProductById(
            [FromRoute] string id,
            CancellationToken cancellationToken)
        {
            var query = new GetProductByIdQuery(id);
            var result = await _mediator.Send(query, cancellationToken);

            if (result.IsFailure)
            {
                return result.Error == Error.NotFound ? NotFound() : Problem(result.Error.Name);
            }

            return Ok(result.Value);
        }

        private IActionResult CommandFailure(Error error)
        {
            if (error == Error.NotFound)
            {
                return NotFound();
            }

            var statusCode = error.Code == ProductErrors.SkuCodeConflictCode
                ? StatusCodes.Status409Conflict
                : StatusCodes.Status400BadRequest;

            return Problem(title: error.Code, detail: error.Name, statusCode: statusCode);
        }
    }
}
