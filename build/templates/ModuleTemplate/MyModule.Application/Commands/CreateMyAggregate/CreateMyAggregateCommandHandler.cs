using MediatR;
using MyModule.Domain.Aggregates;
using MyModule.Domain.Interfaces;

namespace MyModule.Application.Commands.CreateMyAggregate;

internal sealed class CreateMyAggregateCommandHandler : IRequestHandler<CreateMyAggregateCommand, Guid>
{
    private readonly IMyAggregateRepository _repository;

    public CreateMyAggregateCommandHandler(IMyAggregateRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(CreateMyAggregateCommand request, CancellationToken cancellationToken)
    {
        // TODO: Add domain logic for creation
        var aggregate = MyAggregate.Create();

        await _repository.AddAsync(aggregate, cancellationToken);

        // In a real-world scenario, you might need to save changes via a Unit of Work.

        return aggregate.Id;
    }
}
