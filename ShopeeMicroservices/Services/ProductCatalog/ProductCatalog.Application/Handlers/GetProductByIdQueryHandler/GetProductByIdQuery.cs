using Infrastructure.Abstractions.Messaging;
using ProductCatalog.Application.DTOs;

namespace ProductCatalog.Application.Handlers.GetProductByIdQueryHandler
{
    public record GetProductByIdQuery(string Id) : IQuery<ProductDetailsDto>;
}
