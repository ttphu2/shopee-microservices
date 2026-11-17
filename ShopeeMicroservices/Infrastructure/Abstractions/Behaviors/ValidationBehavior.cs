using FluentValidation;
using Infrastructure.Abstractions.Messaging;
using Infrastructure.Errors;
using MediatR;

namespace Infrastructure.Abstractions.Behaviors;

public class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseCommand
    where TResponse : ValidationOutcome, new()
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        var errors = _validators
            .Select(validator => validator.Validate(context))
            .SelectMany(validationResult => validationResult.Errors)
            .Select(ValidationMessages.ToError)
            .ToList();

        if (errors.Count > 0)
        {
            return new TResponse { ValidationMessages = new ValidationMessages(errors) };
        }

        return await next();
    }
}
