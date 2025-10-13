using FluentAssertions;
using PineConePro.Erp.PIM.Domain.Aggregates;

namespace PineConePro.Erp.PIM.Domain.Tests.Aggregates;

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
        var aggregate = MyAggregate.Create("Test Aggregate");

        // Assert
        aggregate.Should().NotBeNull();
        aggregate.Id.Should().NotBeEmpty();
        aggregate.Name.Should().Be("Test Aggregate");
    }
}
