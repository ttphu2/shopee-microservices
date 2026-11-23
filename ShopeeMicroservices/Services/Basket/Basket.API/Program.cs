using System.Reflection;
using Basket.Application;
using Basket.Infrastructure;
using Basket.Infrastructure.Data;
using HealthChecks.UI.Client;
using Infrastructure.Exceptions;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfig) =>
    loggerConfig.ReadFrom.Configuration(context.Configuration));

builder.Services.AddControllers();

builder.Services.AddGlobalExceptionHandler();

builder.Services.AddEndpointsApiExplorer(); // discovers all API endpoints.
builder.Services.AddSwaggerGen();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

var isGeneratingOpenApi = Assembly.GetEntryAssembly()?.GetName().Name == "dotnet-swagger";

// Create the database and its table, only if they do not exist yet.
if (!isGeneratingOpenApi)
{
    await DatabaseInitializer.InitializeAsync(app.Services);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    });
}

app.UseHttpsRedirection();

app.UseSerilogRequestLogging();

app.UseExceptionHandler();

app.MapControllers();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();
