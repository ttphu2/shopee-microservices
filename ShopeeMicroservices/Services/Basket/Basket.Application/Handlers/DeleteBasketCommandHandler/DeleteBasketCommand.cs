using Infrastructure.Abstractions.Messaging;

namespace Basket.Application.Handlers.DeleteBasketCommandHandler
{
    public record DeleteBasketCommand(string UserName) : ICommand;
}
