using Infrastructure.Errors;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Application.DTOs;
using ProductCatalog.Application.Handlers.CreateProductCommandHandler;
using ProductCatalog.Application.Handlers.DeleteProductCommandHandler;
using ProductCatalog.Application.Handlers.GetProductByIdQueryHandler;
using ProductCatalog.Application.Handlers.GetProductQueryHandler;
using ProductCatalog.Application.Handlers.UpdateProductCommandHandler;
using System.Net.Mime;

namespace ProductCatalog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.ProblemJson)]
    public class ProductController(ISender sender) : ControllerBase
    {
        [HttpGet]
        public Task<Validation<GetProductsQueryResult>> GetProducts(CancellationToken cancellationToken)
        {
            return sender.Send(new GetProductsQuery(), cancellationToken);
        }

        [HttpGet("{id}")]
        public Task<Validation<ProductDetailsDto>> GetProductById([FromRoute] string id, CancellationToken cancellationToken)
        {
            return sender.Send(new GetProductByIdQuery(id), cancellationToken);
        }

        [HttpPost]
        public Task<Validation<CreateProductResult>> CreateProduct(
            [FromBody] CreateProductRequest createProductRequest,
            CancellationToken cancellationToken)
        {
            var command = createProductRequest.Adapt<CreateProductCommand>();

            return sender.Send(command, cancellationToken);
        }

        [HttpPut("{id}")]
        public Task<ValidationOutcome> UpdateProduct(
            [FromRoute] string id,
            [FromBody] UpdateProductRequest updateProductRequest,
            CancellationToken cancellationToken)
        {
            var command = updateProductRequest.Adapt<UpdateProductCommand>() with { Id = id };

            return sender.Send(command, cancellationToken);
        }

        [HttpDelete("{id}")]
        public Task<ValidationOutcome> DeleteProduct([FromRoute] string id, CancellationToken cancellationToken)
        {
            return sender.Send(new DeleteProductCommand(id), cancellationToken);
        }
    }
}
