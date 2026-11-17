using Infrastructure.Errors;
using MediatR;

namespace Infrastructure.Abstractions.Messaging;

public interface ICommandHandler<TCommand> : IRequestHandler<TCommand, ValidationOutcome>
    where TCommand : ICommand
{
}

public interface ICommandHandler<TCommand, TResponse> : IRequestHandler<TCommand, Validation<TResponse>>
    where TCommand : ICommand<TResponse>
{
}