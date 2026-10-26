using Infrastructure.Abstractions.Messaging;
using ProductCatalog.Application.Abstractions.Clock;
using ProductCatalog.Application.Errors;
using ProductCatalog.Application.Handlers.Base;
using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Entities;
using ProductCatalog.Domain.Repositories;

namespace ProductCatalog.Application.Handlers.UpdateProductCommandHandler
{
    public class UpdateProductCommandHandler : TransactionalCommandHandler<UpdateProductCommand>
    {
        private readonly IProductCatalogRepository _productCatalogRepository;
        private readonly IDateTimeProvider _dateTimeProvider;

        public UpdateProductCommandHandler(
            IUnitOfWork unitOfWork,
            IProductCatalogRepository productCatalogRepository,
            IDateTimeProvider dateTimeProvider) : base(unitOfWork)
        {
            _productCatalogRepository = productCatalogRepository;
            _dateTimeProvider = dateTimeProvider;
        }

        protected override async Task<Result> ExecuteAsync(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _productCatalogRepository.GetProductByIdAsync(request.Id, cancellationToken);

            if (product is null)
            {
                return Result.Failure(Error.NotFound);
            }

            if (await _productCatalogRepository.GetBrandByIdAsync(request.BrandId, cancellationToken) is null)
            {
                return Result.Failure(ProductErrors.BrandNotFound);
            }

            if (await _productCatalogRepository.GetCategoryByIdAsync(request.CategoryId, cancellationToken) is null)
            {
                return Result.Failure(ProductErrors.CategoryNotFound);
            }

            var requestedSkuCodes = request.Skus.Select(sku => sku.SkuCode).ToList();
            var conflictingSkuCodes = (await _productCatalogRepository.GetSkusByCodesAsync(requestedSkuCodes, cancellationToken))
                .Where(sku => sku.SpuId != product.Id)
                .Select(sku => sku.SkuCode)
                .Distinct()
                .ToList();

            if (conflictingSkuCodes.Count > 0)
            {
                return Result.Failure(ProductErrors.SkuCodeConflict(conflictingSkuCodes));
            }

            var modifiedAt = _dateTimeProvider.UtcNow;

            product.Update(
                request.Name,
                request.Description,
                request.BrandId,
                request.CategoryId,
                request.Images,
                request.VariantOptions,
                modifiedAt);

            // Existing SKUs are matched by SkuCode so a kept SKU keeps its Id; unmatched ones are created or deleted.
            // The validator guarantees SkuCodes are unique within the request.
            var requestedSkus = request.Skus.ToDictionary(sku => sku.SkuCode, StringComparer.Ordinal);
            var matchedSkuCodes = new HashSet<string>(StringComparer.Ordinal);
            var updatedSkus = new List<ProductSku>();
            var removedSkuIds = new List<string>();

            foreach (var existingSku in await _productCatalogRepository.GetSkusByProductIdAsync(product.Id, cancellationToken))
            {
                if (requestedSkus.TryGetValue(existingSku.SkuCode, out var requestedSku) && matchedSkuCodes.Add(existingSku.SkuCode))
                {
                    existingSku.Update(requestedSku.Price, requestedSku.ImageUrl, requestedSku.Attributes, modifiedAt);
                    updatedSkus.Add(existingSku);
                }
                else
                {
                    removedSkuIds.Add(existingSku.Id);
                }
            }

            var addedSkus = request.Skus
                .Where(sku => !matchedSkuCodes.Contains(sku.SkuCode))
                .Select(sku => ProductSku.Create(product.Id, sku.SkuCode, sku.Price, sku.ImageUrl, sku.Attributes))
                .ToList();

            await _productCatalogRepository.UpdateProductAsync(product, cancellationToken);
            await _productCatalogRepository.UpdateSkusAsync(updatedSkus, cancellationToken);
            await _productCatalogRepository.CreateSkusAsync(addedSkus, cancellationToken);
            await _productCatalogRepository.DeleteSkusAsync(removedSkuIds, cancellationToken);

            return Result.Success();
        }
    }
}
