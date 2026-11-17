using System.Reflection;
using HealthChecks.UI.Client;
using Infrastructure.Exceptions;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using ProductCatalog.API.Extensions;
using ProductCatalog.Application;
using ProductCatalog.Infrastructure;
using ProductCatalog.Infrastructure.Data;
using ProductCatalog.Infrastructure.Settings;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfig) =>
    loggerConfig.ReadFrom.Configuration(context.Configuration));

BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));

builder.Services.AddControllers();

builder.Services.AddGlobalExceptionHandler();

builder.Services.AddEndpointsApiExplorer(); // discovers all API endpoints.
builder.Services.AddSwaggerGen();

builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("DatabaseSettings"));
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

var isGeneratingOpenApi = Assembly.GetEntryAssembly()?.GetName().Name == "dotnet-swagger";

// Seed the database and create its indexes, only if the database does not exist yet.
if (!isGeneratingOpenApi)
{
    await DatabaseInitializer.InitializeAsync(
        app.Services.GetRequiredService<IMongoClient>(),
        app.Services.GetRequiredService<IOptions<DatabaseSettings>>(),
        app.Logger);
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

app.UseRequestContextLogging();

app.UseSerilogRequestLogging();

app.UseExceptionHandler();

//app.UseAuthentication();

//app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();
