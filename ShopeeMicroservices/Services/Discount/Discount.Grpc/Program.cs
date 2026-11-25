using Discount.Application;
using Discount.Grpc.Services;
using Discount.Infrastructure;
using Discount.Infrastructure.Data;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfig) =>
    loggerConfig.ReadFrom.Configuration(context.Configuration));

builder.Services.AddGrpc();
builder.Services.AddGrpcReflection(); // lets tools such as grpcurl and Postman discover the service.

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Apply pending migrations; a new database also gets the seed coupons.
await DatabaseInitializer.InitializeAsync(app.Services);

app.UseSerilogRequestLogging();

app.MapGrpcService<DiscountService>();

if (app.Environment.IsDevelopment())
{
    app.MapGrpcReflectionService();
}

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();
