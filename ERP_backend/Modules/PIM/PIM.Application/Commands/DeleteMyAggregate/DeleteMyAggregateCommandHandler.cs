using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PIM.Domain.Interfaces;

namespace PIM.Application.Commands.DeleteMyAggregate;

internal sealed class DeleteMyAggregateCommandHandler : IRequestHandler<DeleteMyAggregateCommand, bool>
{
    private readonly IMyAggregateRepository _repository;

    public DeleteMyAggregateCommandHandler(IMyAggregateRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(DeleteMyAggregateCommand request, CancellationToken cancellationToken)
    {
        var aggregate = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (aggregate is null)
        {
            return false;
        }

        await _repository.DeleteAsync(aggregate, cancellationToken);

        // In a real-world scenario, you might need to save changes via a Unit of Work.

        return true;
    }
}
