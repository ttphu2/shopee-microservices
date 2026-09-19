using Infrastructure.Abstractions.Messaging;
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
            var brand = await _productCatalogRepository.GetBrandByIdAsync(request.BrandId);
            var type = await _productCatalogRepository.GetCategoryByIdAsync(request.CategoryId);

            if (brand is null || type is null)
            {
                throw new ApplicationException("Invalid Brand or Type Specified");
            }

            var productSpu = ProductSpu.Create(
                request.ProductCode,
                request.Name,
                request.Description,
                request.BrandId,
                request.CategoryId,
                request.Images,
                request.VariantOptions);

            var newProduct = await _productCatalogRepository.CreateProductAsync(productSpu);

            var skus = request.Skus
                .Select(sku => ProductSku.Create(
                    newProduct.Id,
                    sku.SkuCode,
                    sku.Price,
                    sku.ImageUrl,
                    sku.Attributes))
                .ToList();

            await _productCatalogRepository.CreateSkusAsync(skus);

            return Result.Success(new CreateProductResult());
        }
    }
}
