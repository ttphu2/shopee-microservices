using Infrastructure.Abstractions.Messaging;
using Microsoft.Extensions.Logging;
using ProductCatalog.Domain.Repositories;

namespace ProductCatalog.Application.Handlers.GetProductByIdQueryHandler
{
    public class GetProductByIdQueryHandler
        (IProductCatalogRepository productCatalogRepository, ILogger<GetProductByIdQueryHandler> logger)
        : IQueryHandler<GetProductByIdQuery, GetProductByIdQueryResult>
    {
        public async Task<Result<GetProductByIdQueryResult>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            logger.LogInformation("GetProductsByIdQueryHandler called with {@request}", request);

            var product = await productCatalogRepository.GetProductWithSkusAsync(request.Id, cancellationToken);

            if (product is null)
            {
                return Result.Failure<GetProductByIdQueryResult>(Error.NotFound);
            }

            return new GetProductByIdQueryResult(product);
        }
    }
}
