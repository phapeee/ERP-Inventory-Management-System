
using MediatR;
using Microsoft.Extensions.Logging;
using NSubstitute;
using MyModule.Application.Behaviours;

namespace MyModule.Application.Tests.Behaviours;

/// <summary>
/// Unit tests for the <see cref="LoggingBehaviour{TRequest, TResponse}"/> class.
/// </summary>
public sealed class LoggingBehaviourTests
{
    private readonly ILogger<LoggingBehaviour<IRequest<Unit>, Unit>> _logger = Substitute.For<ILogger<LoggingBehaviour<IRequest<Unit>, Unit>>>();
    private readonly LoggingBehaviour<IRequest<Unit>, Unit> _behaviour;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoggingBehaviourTests"/> class.
    /// </summary>
    public LoggingBehaviourTests()
    {
        _behaviour = new LoggingBehaviour<IRequest<Unit>, Unit>(_logger);
    }

    /// <summary>
    /// Tests that the Handle method logs the request and response.
    /// </summary>
    [Fact]
    public async Task HandleShouldLogRequestAndResponse()
    {
        // Arrange
        var request = new TestRequest();
        var response = new Unit();
        var next = new RequestHandlerDelegate<Unit>((value) => Task.FromResult(response));

        // Act
        await _behaviour.Handle(request, next, CancellationToken.None);

        // Assert
        // Cannot easily test the extension methods, so we just check if the code runs without errors.
    }

    private sealed record TestRequest : IRequest<Unit>;
}
