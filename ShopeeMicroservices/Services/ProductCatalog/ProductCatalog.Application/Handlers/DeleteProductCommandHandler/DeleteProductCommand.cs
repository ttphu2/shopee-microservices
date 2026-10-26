using Infrastructure.Abstractions.Messaging;

namespace ProductCatalog.Application.Handlers.DeleteProductCommandHandler
{
    public record DeleteProductCommand(string Id) : ICommand;
}
