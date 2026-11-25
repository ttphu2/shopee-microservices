using Discount.Domain.Repositories;
using Discount.Infrastructure.Data;
using Discount.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Discount.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var databaseConnectionString = configuration.GetConnectionString("Database")
            ?? throw new InvalidOperationException("Connection string 'Database' is not configured.");

        services.AddDbContext<DiscountDbContext>(options => options.UseNpgsql(databaseConnectionString));

        services.AddScoped<ICouponRepository, CouponRepository>();

        // Reported by the /health endpoint.
        services.AddHealthChecks()
            .AddNpgSql(databaseConnectionString);

        return services;
    }
}
