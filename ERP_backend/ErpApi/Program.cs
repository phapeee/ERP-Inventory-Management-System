using ErpApi.Configuration;
using ErpApi.Data.Generated;
using ErpApi.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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

// Setup link: https://www.c-sharpcorner.com/article/building-a-powerful-asp-net-core-web-api-with-postgresql/
// Offical setup link: https://www.npgsql.org/efcore/index.html?tabs=onconfiguring
// Use .NET (C#) to connect and query data in Azure Database for PostgreSQL flexible server: https://learn.microsoft.com/en-us/azure/postgresql/flexible-server/connect-csharp?utm_source=chatgpt.com
builder.Services.AddDbContext<AppDb>(opts =>
    opts.UseNpgsql(connectionString));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

await app.EnsureDatabaseConnectionAsync(connectionString);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();  // UI at /scalar
}

app.UseHttpsRedirection();

app.MapGet("/health", () => Results.Ok("ok"));

app.MapControllers();
await app.RunAsync();
