using System;
using Microsoft.Extensions.Logging;

namespace ErpApi.Logging;

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
