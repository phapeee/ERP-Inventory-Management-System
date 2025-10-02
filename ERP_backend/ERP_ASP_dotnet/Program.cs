using ERP_backend.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using Scalar.AspNetCore;

LoadDotEnv();

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

await EnsureDatabaseConnectionAsync(app, connectionString);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();  // UI at /scalar
}

app.UseHttpsRedirection();

app.MapControllers();
app.Run();

static async Task EnsureDatabaseConnectionAsync(WebApplication app, string connectionString)
{
    await using var scope = app.Services.CreateAsyncScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDb>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    logger.LogInformation("Attempting database connection using {ConnectionInfo}",
        GetSafeConnectionInfo(connectionString));

    try
    {
        if (!await dbContext.Database.CanConnectAsync())
        {
            const string message = "Unable to connect to the database using the configured connection string.";
            logger.LogCritical(message);
            throw new InvalidOperationException(message);
        }

        logger.LogInformation("Database connection verified successfully.");
    }
    catch (Exception ex)
    {
        const string message = "Unable to connect to the database using the configured connection string.";
        logger.LogCritical(ex, message);
        throw new InvalidOperationException(message, ex);
    }

    static string GetSafeConnectionInfo(string connectionString)
    {
        try
        {
            var builder = new NpgsqlConnectionStringBuilder(connectionString);
            return $"Host={builder.Host};Port={builder.Port};Database={builder.Database};Username={builder.Username}";
        }
        catch
        {
            return "[unparsable connection string]";
        }
    }
}

static void LoadDotEnv()
{
    var envPath = Path.Combine(Directory.GetCurrentDirectory(), ".env");
    if (!File.Exists(envPath))
    {
        return;
    }

    foreach (var line in File.ReadLines(envPath))
    {
        var trimmed = line.Trim();

        if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith("#", StringComparison.Ordinal))
        {
            continue;
        }

        var separatorIndex = trimmed.IndexOf('=');
        if (separatorIndex <= 0)
        {
            continue;
        }

        var key = trimmed[..separatorIndex].Trim();
        if (key.Length == 0)
        {
            continue;
        }

        var value = trimmed[(separatorIndex + 1)..].Trim();
        if (value.Length >= 2 && value.StartsWith('"') && value.EndsWith('"'))
        {
            value = value[1..^1];
        }

        Environment.SetEnvironmentVariable(key, value);
    }
}
