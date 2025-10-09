using FluentAssertions;
using MyModule.Domain.Aggregates;

namespace MyModule.Domain.Tests.Aggregates;

/// <summary>
/// Unit tests for the <see cref="MyAggregate"/> class.
/// </summary>
public class MyAggregateTests
{
    /// <summary>
    /// Tests that the <see cref="MyAggregate.Create"/> method returns a new aggregate.
    /// </summary>
    [Fact]
    public void CreateShouldReturnNewAggregateWhenCalled()
    {
        // Act
        var aggregate = MyAggregate.Create();

        // Assert
        aggregate.Should().NotBeNull();
        aggregate.Id.Should().NotBeEmpty();
    }
}
