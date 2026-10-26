using Infrastructure.Abstractions.Messaging;
using ProductCatalog.Application.Handlers.Base;
using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Repositories;

namespace ProductCatalog.Application.Handlers.DeleteProductCommandHandler
{
    // Deletes the product and all of its SKUs in one transaction.
    public class DeleteProductCommandHandler : TransactionalCommandHandler<DeleteProductCommand>
    {
        private readonly IProductCatalogRepository _productCatalogRepository;

        public DeleteProductCommandHandler(IUnitOfWork unitOfWork, IProductCatalogRepository productCatalogRepository) : base(unitOfWork)
        {
            _productCatalogRepository = productCatalogRepository;
        }

        protected override async Task<Result> ExecuteAsync(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _productCatalogRepository.GetProductByIdAsync(request.Id, cancellationToken);

            if (product is null)
            {
                return Result.Failure(Error.NotFound);
            }

            await _productCatalogRepository.DeleteSkusByProductIdAsync(product.Id, cancellationToken);
            await _productCatalogRepository.DeleteProductAsync(product.Id, cancellationToken);

            return Result.Success();
        }
    }
}
