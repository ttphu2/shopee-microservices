namespace Infrastructure.Errors;

public class ValidationOutcome
{
    public ValidationOutcome()
    {
    }

    public ValidationOutcome(Error error, params Error[] moreErrors)
        : this(new ValidationMessages(error, moreErrors))
    {
    }

    public ValidationOutcome(ValidationMessages validationMessages)
    {
        ValidationMessages = validationMessages;
    }

    public bool IsSuccessful => ValidationMessages == null;

    public ValidationMessages? ValidationMessages { get; init; }

    public static ValidationOutcome Success() => new();

    public static ValidationOutcome Failure(Error error, params Error[] moreErrors) => new(error, moreErrors);

    public static Validation<TValue> Success<TValue>(TValue value) => new(value);

    public static Validation<TValue> Failure<TValue>(Error error, params Error[] moreErrors) =>
        new(new ValidationMessages(error, moreErrors));

    public static Validation<TValue> Create<TValue>(TValue? value) =>
        value is not null ? Success(value) : Failure<TValue>(Error.NullValue);
}

public class Validation<TValue> : ValidationOutcome
{
    public Validation()
    {
    }

    public Validation(TValue value)
    {
        Value = value;
    }

    public Validation(ValidationMessages validationMessages)
        : base(validationMessages)
    {
    }

    public TValue? Value { get; }

    public static implicit operator Validation<TValue>(TValue? value) => Create(value);

    public static implicit operator Validation<TValue>(Error error) => Failure<TValue>(error);
}
