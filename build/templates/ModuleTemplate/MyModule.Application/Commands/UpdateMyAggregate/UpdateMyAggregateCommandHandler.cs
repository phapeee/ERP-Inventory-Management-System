using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PineConePro.Erp.MyModule.Domain.Interfaces;

namespace PineConePro.Erp.MyModule.Application.Commands.UpdateMyAggregate;

internal sealed class UpdateMyAggregateCommandHandler : IRequestHandler<UpdateMyAggregateCommand, bool>
{
    private readonly IMyAggregateRepository _repository;

    public UpdateMyAggregateCommandHandler(IMyAggregateRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(UpdateMyAggregateCommand request, CancellationToken cancellationToken)
    {
        var aggregate = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (aggregate is null)
        {
            return false;
        }

        aggregate.UpdateName(request.Name);
        await _repository.UpdateAsync(aggregate, cancellationToken);

        // In a real-world scenario, you might need to save changes via a Unit of Work.

        return true;
    }
}
