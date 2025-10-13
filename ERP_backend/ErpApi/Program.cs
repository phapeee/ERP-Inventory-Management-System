using System.Net.Http;
using ErpApi.Configuration;
using ErpApi.Data.Generated;
using ErpApi.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PineConePro.Erp.PIM.Presentation.Modules;
using Scalar.AspNetCore;

EnvironmentVariableLoader.LoadDotEnv(System.IO.Directory.GetCurrentDirectory());

var builder = WebApplication.CreateBuilder(args);

var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("Environment variable 'DB_CONNECTION_STRING' must be set.");
}

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// Launch the app and navigate to https://localhost:<port>/openapi/v1.json to view the generated OpenAPI document.
builder.Services.AddOpenApi();
builder.Services.AddApplicationInsightsTelemetry();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IModuleRegistry, ModuleRegistry>();

// Setup link: https://www.c-sharpcorner.com/article/building-a-powerful-asp-net-core-web-api-with-postgresql/
// Offical setup link: https://www.npgsql.org/efcore/index.html?tabs=onconfiguring
// Use .NET (C#) to connect and query data in Azure Database for PostgreSQL flexible server: https://learn.microsoft.com/en-us/azure/postgresql/flexible-server/connect-csharp?utm_source=chatgpt.com
builder.Services.AddDbContext<AppDb>(opts =>
    opts.UseNpgsql(connectionString));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.InstallDiscoveredModules(builder.Configuration);

var app = builder.Build();

await app.EnsureDatabaseConnectionAsync(connectionString);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();  // UI at /scalar
}

app.UseHttpsRedirection();

app.MapGet("/health", async (IHttpClientFactory httpClientFactory, IModuleRegistry moduleRegistry, ILogger<IModuleRegistry> logger) =>
{
    var client = httpClientFactory.CreateClient();
    var results = new Dictionary<string, bool>();
    var baseUrl = "http://localhost:5000"; // Use a fixed, trusted URL

    foreach (var module in moduleRegistry.Modules)
    {
        var url = baseUrl + module.HealthEndpointUrl;
        logger.LogInformation("Checking health of module {ModuleName} at {Url}", module.Name, url);
        try
        {
            var response = await client.GetAsync(url);
            results[module.Name] = response.IsSuccessStatusCode;
            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning("Health check for module {ModuleName} failed with status code {StatusCode}", module.Name, response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Health check for module {ModuleName} failed with an exception.", module.Name);
            results[module.Name] = false;
        }
    }

    if (results.All(r => r.Value))
    {
        return Results.Ok(results);
    }
    else
    {
        var extensions = new Dictionary<string, object?> { { "modules", results } };
        return Results.Problem(detail: "One or more modules are unhealthy.", statusCode: 503, extensions: extensions);
    }
});

app.MapControllers();
app.MapDiscoveredModules();

await app.RunAsync();
