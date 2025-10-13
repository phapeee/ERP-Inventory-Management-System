using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PineConePro.Erp.TaxReporting.Application.Dtos;
using PineConePro.Erp.TaxReporting.Domain.Interfaces;

namespace PineConePro.Erp.TaxReporting.Application.Queries.GetMyAggregateById;

internal sealed class GetMyAggregateByIdQueryHandler : IRequestHandler<GetMyAggregateByIdQuery, MyAggregateDto?>
{
    private readonly IMyAggregateRepository _repository;

    public GetMyAggregateByIdQueryHandler(IMyAggregateRepository repository)
    {
        _repository = repository;
    }

    public async Task<MyAggregateDto?> Handle(GetMyAggregateByIdQuery request, CancellationToken cancellationToken)
    {
        var aggregate = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (aggregate is null)
        {
            return null;
        }

        // TODO: Use a mapper (like AutoMapper) to map the aggregate to a DTO
        return new MyAggregateDto(aggregate.Id, aggregate.Name);
    }
}
