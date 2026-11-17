using FluentValidation.Results;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Infrastructure.Errors;

[SuppressMessage("maintainability", "S3877:Exceptions should not be thrown from unexpected methods")]
public record ValidationMessages
{
    private readonly List<Error> _errors = [];

    public ValidationMessages(IReadOnlyList<Error> errors)
    {
        if (!errors.Any())
        {
            throw new ArgumentException("Provide at least one validation message.");
        }
        _errors.AddRange(errors);
    }

    public ValidationMessages(Error error, params Error[] errors)
    {
        _errors.Add(error);
        _errors.AddRange(errors);
    }

    public static implicit operator ValidationMessages(ValidationResult validationResult)
    {
        if (validationResult.IsValid)
        {
            throw new ArgumentException("Cannot create from valid ValidationResult.");
        }
        return new ValidationMessages(validationResult.Errors.Select(ToError).ToList());
    }

    public IReadOnlyList<string> Messages => _errors.Select(err => err.Message).ToImmutableList();

    internal static Error ToError(ValidationFailure failure) => new(failure.ErrorCode ?? failure.PropertyName, failure.ErrorMessage);
}