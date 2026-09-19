using ProductCatalog.Application.Abstractions.Clock;

namespace ProductCatalog.Infrastructure.Clock;

internal sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}