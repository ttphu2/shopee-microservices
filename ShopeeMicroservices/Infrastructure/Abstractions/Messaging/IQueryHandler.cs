using Infrastructure.Errors;
using MediatR;

namespace Infrastructure.Abstractions.Messaging;

public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, Validation<TResponse>>
    where TQuery : IQuery<TResponse>
{
}