using FluentAssertions;
using MyModule.Domain.Entities;

namespace MyModule.Domain.Tests.Entities;

/// <summary>
/// Unit tests for the <see cref="MyEntity"/> class.
/// </summary>
public class MyEntityTests
{
    /// <summary>
    /// Tests that the <see cref="MyEntity.Create"/> method returns a new entity with the given name.
    /// </summary>
    [Fact]
    public void CreateShouldReturnNewEntityWithGivenName()
    {
        // Arrange
        var name = "Test Entity";

        // Act
        var entity = MyEntity.Create(name);

        // Assert
        entity.Should().NotBeNull();
        entity.Id.Should().NotBeEmpty();
        entity.Name.Should().Be(name);
    }
}
