using Infrastructure.Abstractions.Messaging;
using Microsoft.Extensions.Logging;
using ProductCatalog.Application.Mappers;
using ProductCatalog.Domain.Repositories;

namespace ProductCatalog.Application.Handlers.GetProductQueryHandler
{
    public record GetProductsQueryHandler
        (IProductCatalogRepository productCatalogRepository, ILogger<GetProductsQueryHandler> logger)
        : IQueryHandler<GetProductsQuery, GetProductsQueryResult>
    {
        public async Task<Result<GetProductsQueryResult>> Handle(GetProductsQuery query, CancellationToken cancellationToken)
        {
            logger.LogInformation("GetProductsQueryHandler called with {@query}", query);

            var products = await productCatalogRepository.GetAllAsync();

            return new GetProductsQueryResult(products.ToDtoList());
        }
    }
}
