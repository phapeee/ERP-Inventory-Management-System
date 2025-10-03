using ErpApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using Scalar.AspNetCore;
using System.Data.Common;

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

app.MapGet("/health", () => Results.Ok("ok"));

app.MapControllers();
await app.RunAsync();

static async Task EnsureDatabaseConnectionAsync(WebApplication app, string connectionString)
{
    await using var scope = app.Services.CreateAsyncScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDb>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    const string ConnectionFailureMessage = "Unable to connect to the database using the configured connection string.";

    ConnectionLogging.AttemptingDatabaseConnection(logger, GetSafeConnectionInfo(connectionString));

    bool canConnect;

    try
    {
        canConnect = await dbContext.Database.CanConnectAsync();
    }
    catch (DbException ex)
    {
        throw CreateDatabaseConnectionException(logger, ConnectionFailureMessage, ex);
    }
    catch (InvalidOperationException ex)
    {
        throw CreateDatabaseConnectionException(logger, ConnectionFailureMessage, ex);
    }

    if (!canConnect)
    {
        ConnectionLogging.DatabaseConnectionVerificationFailed(logger);
        throw new InvalidOperationException(ConnectionFailureMessage);
    }

    ConnectionLogging.DatabaseConnectionVerified(logger);

    static string GetSafeConnectionInfo(string connectionString)
    {
        try
        {
            var builder = new NpgsqlConnectionStringBuilder(connectionString);
            return $"Host={builder.Host};Port={builder.Port};Database={builder.Database};Username={builder.Username}";
        }
        catch (Exception ex) when (ex is ArgumentException or FormatException or InvalidOperationException)
        {
            return "[unparsable connection string]";
        }
    }

    static InvalidOperationException CreateDatabaseConnectionException(ILogger logger, string message, Exception ex)
    {
        ConnectionLogging.DatabaseConnectionException(logger, ex);
        return new InvalidOperationException(message, ex);
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

        if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith('#'))
        {
            continue;
        }

        var separatorIndex = trimmed.IndexOf('=', StringComparison.Ordinal);
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

internal static partial class ConnectionLogging
{
    [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "Attempting database connection using {ConnectionInfo}")]
    public static partial void AttemptingDatabaseConnection(ILogger logger, string connectionInfo);

    [LoggerMessage(EventId = 2, Level = LogLevel.Critical, Message = "Unable to connect to the database using the configured connection string.")]
    public static partial void DatabaseConnectionVerificationFailed(ILogger logger);

    [LoggerMessage(EventId = 3, Level = LogLevel.Information, Message = "Database connection verified successfully.")]
    public static partial void DatabaseConnectionVerified(ILogger logger);

    [LoggerMessage(EventId = 4, Level = LogLevel.Critical, Message = "Unable to connect to the database using the configured connection string.")]
    public static partial void DatabaseConnectionException(ILogger logger, Exception exception);
}
