using System;
using System.Data.Common;
using ErpApi.Data.Generated;
using ErpApi.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace ErpApi.Hosting;

internal static class WebApplicationDatabaseExtensions
{
    public static async Task EnsureDatabaseConnectionAsync(this WebApplication app, string connectionString)
    {
        ArgumentNullException.ThrowIfNull(app);

        const string ConnectionFailureMessage = "Unable to connect to the database using the configured connection string.";

        await using var scope = app.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDb>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

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
    }

    private static string GetSafeConnectionInfo(string connectionString)
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

    private static InvalidOperationException CreateDatabaseConnectionException(ILogger logger, string message, Exception ex)
    {
        ConnectionLogging.DatabaseConnectionException(logger, ex);
        return new InvalidOperationException(message, ex);
    }
}
