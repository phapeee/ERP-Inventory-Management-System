using FluentValidation;

namespace MyModule.Application.Commands.CreateMyAggregate;

internal sealed class CreateMyAggregateCommandValidator : AbstractValidator<CreateMyAggregateCommand>
{
    public CreateMyAggregateCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");
    }
}
