using Infrastructure.Errors;
using MediatR;

namespace Infrastructure.Abstractions.Messaging;

public interface ICommand : IRequest<ValidationOutcome>, IBaseCommand
{
}

public interface ICommand<TReponse> : IRequest<Validation<TReponse>>, IBaseCommand
{
}

public interface IBaseCommand
{
}