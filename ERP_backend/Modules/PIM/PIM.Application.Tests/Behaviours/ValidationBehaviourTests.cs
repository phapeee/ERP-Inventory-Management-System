
using FluentValidation;
using MediatR;
using PIM.Application.Behaviours;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PIM.Application.Tests.Behaviours;

/// <summary>
/// Unit tests for the <see cref="ValidationBehaviour{TRequest, TResponse}"/> class.
/// </summary>
public sealed class ValidationBehaviourTests
{
    /// <summary>
    /// Tests that the Handle method calls the validator.
    /// </summary>
    [Fact]
    public async Task HandleShouldCallValidator()
    {
        // Arrange
        var validator = new TestValidator();
        var validators = new List<IValidator<TestRequest>> { validator };
        var behaviour = new ValidationBehaviour<TestRequest, Unit>(validators);
        var request = new TestRequest();
        var next = new RequestHandlerDelegate<Unit>((value) => Task.FromResult(Unit.Value));

        // Act
        await behaviour.Handle(request, next, CancellationToken.None);

        // Assert
        Assert.True(validator.WasCalled);
    }

    private sealed record TestRequest : IRequest<Unit>;

    private sealed class TestValidator : AbstractValidator<TestRequest>
    {
        public bool WasCalled { get; private set; }

        public TestValidator()
        {
            RuleFor(x => x).Custom((_, context) => WasCalled = true);
        }
    }
}
