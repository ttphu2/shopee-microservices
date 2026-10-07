using Infrastructure.Abstractions.Messaging;

namespace ProductCatalog.Application.Handlers.GetProductQueryHandler
{
    public record GetProductsQuery() : IQuery<GetProductsQueryResult>;
}
