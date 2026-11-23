using Basket.Domain.Repositories;
using Basket.Infrastructure.Data;
using Basket.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Basket.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var databaseConnectionString = configuration.GetConnectionString("Database")
            ?? throw new InvalidOperationException("Connection string 'Database' is not configured.");
        var redisConnectionString = configuration.GetConnectionString("Redis")
            ?? throw new InvalidOperationException("Connection string 'Redis' is not configured.");

        services.AddDbContext<BasketDbContext>(options => options.UseNpgsql(databaseConnectionString));

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnectionString;
            options.InstanceName = "basket:"; // Redis key = "basket:<userName>"
        });

        // Handlers receive the cache-aside decorator, which wraps the PostgreSQL repository.
        services.AddScoped<BasketRepository>();
        services.AddScoped<IBasketRepository>(serviceProvider => new CachedBasketRepository(
            serviceProvider.GetRequiredService<BasketRepository>(),
            serviceProvider.GetRequiredService<IDistributedCache>()));

        // Reported by the /health endpoint.
        services.AddHealthChecks()
            .AddNpgSql(databaseConnectionString)
            .AddRedis(redisConnectionString);

        return services;
    }
}
