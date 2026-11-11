namespace Infrastructure.Abstractions.Messaging;

public record Error(string Code, string Name)
{
    public static readonly Error None = new(string.Empty, string.Empty);

    public static readonly Error NullValue = new("Error.NullValue", "Null value was provided");

    public static readonly Error NotFound = new("Error.NotFound", "The requested resource was not found.");

    // Lets handlers returning Result write "return ProductErrors.BrandNotFound;".
    // The Result<TValue> conversion lives in Result<TValue>, because a conversion operator cannot be generic.
    public static implicit operator Result(Error error) => Result.Failure(error);
}
