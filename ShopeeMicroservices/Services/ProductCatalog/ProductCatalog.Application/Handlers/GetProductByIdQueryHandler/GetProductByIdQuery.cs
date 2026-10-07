using Infrastructure.Abstractions.Messaging;

namespace ProductCatalog.Application.Handlers.GetProductByIdQueryHandler
{
    public record GetProductByIdQuery(string Id) : IQuery<GetProductByIdQueryResult>;
}
