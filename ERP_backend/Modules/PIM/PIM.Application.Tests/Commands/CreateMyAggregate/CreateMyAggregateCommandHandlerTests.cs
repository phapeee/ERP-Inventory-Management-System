
using FluentAssertions;
using MediatR;
using NSubstitute;
using PIM.Application.Commands.CreateMyAggregate;
using PIM.Domain.Aggregates;
using PIM.Domain.Interfaces;

namespace PIM.Application.Tests.Commands.CreateMyAggregate;

/// <summary>
/// Unit tests for the <see cref="CreateMyAggregateCommandHandler"/> class.
/// </summary>
public sealed class CreateMyAggregateCommandHandlerTests
{
    private readonly IMyAggregateRepository _repository = Substitute.For<IMyAggregateRepository>();
    private readonly CreateMyAggregateCommandHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateMyAggregateCommandHandlerTests"/> class.
    /// </summary>
    public CreateMyAggregateCommandHandlerTests()
    {
        _handler = new CreateMyAggregateCommandHandler(_repository);
    }

    /// <summary>
    /// Tests that the Handle method returns a new Guid.
    /// </summary>
    [Fact]
    public async Task HandleShouldReturnNewGuid()
    {
        // Arrange
        var command = new CreateMyAggregateCommand("Test");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty();
        await _repository.Received(1).AddAsync(Arg.Any<MyAggregate>(), CancellationToken.None);
    }
}
