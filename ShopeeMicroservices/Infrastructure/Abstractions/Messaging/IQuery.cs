using Infrastructure.Errors;
using MediatR;

namespace Infrastructure.Abstractions.Messaging;

public interface IQuery<TResponse> : IRequest<Validation<TResponse>>
{
}