using Infrastructure.Abstractions.Messaging;
using ProductCatalog.Application.Errors;
using ProductCatalog.Application.Handlers.Base;
using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Entities;
using ProductCatalog.Domain.Repositories;

namespace ProductCatalog.Application.Handlers.CreateProductCommandHandler
{
    public class CreateProductCommandHandler : TransactionalCommandHandler<CreateProductCommand, CreateProductResult>
    {
        private readonly IProductCatalogRepository _productCatalogRepository;

        public CreateProductCommandHandler(IUnitOfWork unitOfWork, IProductCatalogRepository productCatalogRepository) : base(unitOfWork)
        {
            _productCatalogRepository = productCatalogRepository;
        }

        protected override async Task<Result<CreateProductResult>> ExecuteAsync(CreateProductCommand request, CancellationToken cancellationToken)
        {
            if (await _productCatalogRepository.GetBrandByIdAsync(request.BrandId, cancellationToken) is null)
            {
                return Result.Failure<CreateProductResult>(ProductErrors.BrandNotFound);
            }

            if (await _productCatalogRepository.GetCategoryByIdAsync(request.CategoryId, cancellationToken) is null)
            {
                return Result.Failure<CreateProductResult>(ProductErrors.CategoryNotFound);
            }

            var requestedSkuCodes = request.Skus.Select(sku => sku.SkuCode).ToList();
            var conflictingSkuCodes = (await _productCatalogRepository.GetSkusByCodesAsync(requestedSkuCodes, cancellationToken))
                .Select(sku => sku.SkuCode)
                .Distinct()
                .ToList();

            if (conflictingSkuCodes.Count > 0)
            {
                return Result.Failure<CreateProductResult>(ProductErrors.SkuCodeConflict(conflictingSkuCodes));
            }

            var productSpu = ProductSpu.Create(
                request.ProductCode,
                request.Name,
                request.Description,
                request.BrandId,
                request.CategoryId,
                request.Images,
                request.VariantOptions);

            var newProduct = await _productCatalogRepository.CreateProductAsync(productSpu, cancellationToken);

            var skus = request.Skus
                .Select(sku => ProductSku.Create(
                    newProduct.Id,
                    sku.SkuCode,
                    sku.Price,
                    sku.ImageUrl,
                    sku.Attributes))
                .ToList();

            await _productCatalogRepository.CreateSkusAsync(skus, cancellationToken);

            return Result.Success(new CreateProductResult());
        }
    }
}
