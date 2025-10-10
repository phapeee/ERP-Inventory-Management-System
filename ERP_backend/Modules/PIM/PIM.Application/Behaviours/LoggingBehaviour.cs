using MediatR;
using Microsoft.Extensions.Logging;

namespace PIM.Application.Behaviours;

internal static partial class LoggingBehaviourLogMessages
{
    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Information,
        Message = "Handling {RequestName}")]
    public static partial void HandlingRequest(this ILogger logger, string requestName);

    [LoggerMessage(
        EventId = 2,
        Level = LogLevel.Information,
        Message = "Handled {RequestName}")]
    public static partial void HandledRequest(this ILogger logger, string requestName);
}

internal sealed class LoggingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehaviour<TRequest, TResponse>> _logger;

    public LoggingBehaviour(ILogger<LoggingBehaviour<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _logger.HandlingRequest(typeof(TRequest).Name);

        var response = await next(cancellationToken);

        _logger.HandledRequest(typeof(TRequest).Name);

        return response;
    }
}
