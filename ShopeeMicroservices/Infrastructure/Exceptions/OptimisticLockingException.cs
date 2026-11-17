namespace Infrastructure.Exceptions;

public sealed class OptimisticLockingException : Exception
{
    public OptimisticLockingException(string message)
        : base(message)
    {
    }

    public OptimisticLockingException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
