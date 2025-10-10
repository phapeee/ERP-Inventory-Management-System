
using FluentValidation.TestHelper;
using PineConePro.Erp.MyModule.Application.Commands.CreateMyAggregate;

namespace PineConePro.Erp.MyModule.Application.Tests.Commands.CreateMyAggregate;

/// <summary>
/// Unit tests for the <see cref="CreateMyAggregateCommandValidator"/> class.
/// </summary>
public sealed class CreateMyAggregateCommandValidatorTests
{
    private readonly CreateMyAggregateCommandValidator _validator = new();

    /// <summary>
    /// Tests that the validator has an error when the name is empty.
    /// </summary>
    [Fact]
    public void ShouldHaveErrorWhenNameIsEmpty()
    {
        // Arrange
        var command = new CreateMyAggregateCommand("");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    /// <summary>
    /// Tests that the validator does not have an error when the name is specified.
    /// </summary>
    [Fact]
    public void ShouldNotHaveErrorWhenNameIsSpecified()
    {
        // Arrange
        var command = new CreateMyAggregateCommand("Test Name");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }
}
