using Infrastructure.Abstractions.Messaging;
using ProductCatalog.Application.DTOs;
using ProductCatalog.Application.Mappers;
using ProductCatalog.Domain.Repositories;

namespace ProductCatalog.Application.Handlers.GetProductByIdQueryHandler
{
    public class GetProductByIdQueryHandler(IProductCatalogRepository productCatalogRepository)
        : IQueryHandler<GetProductByIdQuery, ProductDetailsDto>
    {
        public async Task<Result<ProductDetailsDto>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var product = await productCatalogRepository.GetProductByIdAsync(request.Id, cancellationToken);

            if (product is null)
            {
                return Result.Failure<ProductDetailsDto>(Error.NotFound);
            }

            // Independent reads keyed off the product; no session is involved, so running them concurrently is safe.
            var skusTask = productCatalogRepository.GetSkusByProductIdAsync(product.Id, cancellationToken);
            var brandTask = productCatalogRepository.GetBrandByIdAsync(product.BrandId, cancellationToken);
            var typeTask = productCatalogRepository.GetCategoryByIdAsync(product.CategoryId, cancellationToken);

            await Task.WhenAll(skusTask, brandTask, typeTask);

            return product.ToDetailsDto(skusTask.Result, brandTask.Result, typeTask.Result);
        }
    }
}
