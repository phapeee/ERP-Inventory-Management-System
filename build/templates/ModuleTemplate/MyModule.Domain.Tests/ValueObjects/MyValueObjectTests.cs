using FluentAssertions;
using MyModule.Domain.ValueObjects;

namespace MyModule.Domain.Tests.ValueObjects;

/// <summary>
/// Unit tests for the <see cref="MyValueObject"/> class.
/// </summary>
public class MyValueObjectTests
{
    /// <summary>
    /// Tests that the constructor sets the value correctly.
    /// </summary>
    [Fact]
    public void ConstructorShouldSetValueCorrectly()
    {
        // Arrange
        var value = "TestValue";

        // Act
        var myValueObject = new MyValueObject(value);

        // Assert
        myValueObject.Value.Should().Be(value);
    }

    /// <summary>
    /// Tests that the Equals method returns true when values are equal.
    /// </summary>
    [Fact]
    public void EqualsShouldReturnTrueWhenValuesAreEqual()
    {
        // Arrange
        var value1 = new MyValueObject("Test");
        var value2 = new MyValueObject("Test");

        // Act & Assert
        value1.Should().Be(value2);
    }

    /// <summary>
    /// Tests that the Equals method returns false when values are different.
    /// </summary>
    [Fact]
    public void EqualsShouldReturnFalseWhenValuesAreDifferent()
    {
        // Arrange
        var value1 = new MyValueObject("Test1");
        var value2 = new MyValueObject("Test2");

        // Act & Assert
        value1.Should().NotBe(value2);
    }

    /// <summary>
    /// Tests that GetHashCode returns the same hash code when values are equal.
    /// </summary>
    [Fact]
    public void GetHashCodeShouldReturnSameHashCodeWhenValuesAreEqual()
    {
        // Arrange
        var value1 = new MyValueObject("Test");
        var value2 = new MyValueObject("Test");

        // Act & Assert
        value1.GetHashCode().Should().Be(value2.GetHashCode());
    }
}
