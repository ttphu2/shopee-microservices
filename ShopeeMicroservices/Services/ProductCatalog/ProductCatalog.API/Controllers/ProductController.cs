using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Application.DTOs;
using ProductCatalog.Application.Handlers.CreateProductCommandHandler;
using ProductCatalog.Application.Handlers.GetProductByIdQueryHandler;
using ProductCatalog.Application.Handlers.GetProductQueryHandler;

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
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest createProductRequest)
        {
            var command = createProductRequest.Adapt<CreateProductCommand>();
            var result = await _mediator.Send(command);
            return Ok(result.Value);
        }

        [HttpGet]
        public async Task<ActionResult<GetProductsQueryResult>> GetProducts()
        {
            var query = new GetProductsQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetProductByIdQueryResult>> GetProductById([FromQuery] string id)
        {
            var query = new GetProductByIdQuery(id);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
