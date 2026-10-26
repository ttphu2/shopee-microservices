using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Abstractions.Behaviors;

public static class MediatRConfigurationExtensions
{
    // Order matters: logging wraps validation, so validation failures are logged too.
    public static MediatRServiceConfiguration AddDefaultPipelineBehaviors(
        this MediatRServiceConfiguration configuration)
    {
        configuration.AddOpenBehavior(typeof(LoggingBehavior<,>));

        configuration.AddOpenBehavior(typeof(ValidationBehavior<,>));

        return configuration;
    }
}
