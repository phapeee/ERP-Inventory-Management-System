using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PineConePro.Erp.PIM.Application.Dtos;
using PineConePro.Erp.PIM.Domain.Interfaces;

namespace PineConePro.Erp.PIM.Application.Queries.GetAllMyAggregates;

internal sealed class GetAllMyAggregatesQueryHandler : IRequestHandler<GetAllMyAggregatesQuery, IReadOnlyCollection<MyAggregateDto>>
{
    private readonly IMyAggregateRepository _repository;

    public GetAllMyAggregatesQueryHandler(IMyAggregateRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyCollection<MyAggregateDto>> Handle(GetAllMyAggregatesQuery request, CancellationToken cancellationToken)
    {
        var aggregates = await _repository.GetAllAsync(cancellationToken);

        // TODO: Use a mapper (like AutoMapper) to map the aggregates to DTOs
        return aggregates
            .Select(aggregate => new MyAggregateDto(aggregate.Id, aggregate.Name))
            .ToArray();
    }
}
