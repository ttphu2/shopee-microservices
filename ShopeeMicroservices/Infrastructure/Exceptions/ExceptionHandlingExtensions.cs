using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Exceptions;

public static class ExceptionHandlingExtensions
{
    public static IServiceCollection AddGlobalExceptionHandler(this IServiceCollection services)
    {
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        return services;
    }
}
