using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ProductCatalog.Application.Abstractions.Clock;
using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Repositories;
using ProductCatalog.Infrastructure.Clock;
using ProductCatalog.Infrastructure.Repositories;
using ProductCatalog.Infrastructure.Settings;

namespace ProductCatalog.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddTransient<IDateTimeProvider, DateTimeProvider>();

        AddPersistence(services, configuration);

        return services;
    }

    private static void AddPersistence(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IMongoClient>(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<DatabaseSettings>>().Value;
            return new MongoClient(settings.ConnectionString);
        });

        services.AddScoped<IUnitOfWork, MongoUnitOfWork>();

        services.AddScoped<IProductCatalogRepository, ProductCatalogRepository>();
    }
}