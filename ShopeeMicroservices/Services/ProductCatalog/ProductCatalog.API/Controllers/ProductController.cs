using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Application.DTOs;
using ProductCatalog.Application.Handlers.CreateProductCommandHandler;

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

        // Error bodies are written as application/json by ExceptionHandlingMiddleware.
        [HttpPost]
        [ProducesResponseType(typeof(CreateProductResult), StatusCodes.Status200OK, "application/json")]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest, "application/json")]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError, "application/json")]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest createProductRequest)
        {
            var command = createProductRequest.Adapt<CreateProductCommand>();
            var result = await _mediator.Send(command);
            return Ok(result.Value);
        }
    }
}
