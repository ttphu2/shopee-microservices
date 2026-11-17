namespace Infrastructure.Errors;

public record Error(string Code, string Message)
{
    public static readonly Error NullValue = new("Error.NullValue", "Null value was provided");

    public static readonly Error NotFound = new("Error.NotFound", "The requested resource was not found.");

    public static readonly Error OptimisticLocking = new(
        "Error.OptimisticLocking",
        "This data was changed by someone else in the meantime. Reload it and try again.");

    // Lets handlers returning ValidationOutcome write "return ProductErrors.BrandNotFound;".
    // The Validation<TValue> conversion lives in Validation<TValue>, because a conversion operator cannot be generic.
    public static implicit operator ValidationOutcome(Error error) => ValidationOutcome.Failure(error);
}
