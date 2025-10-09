using MediatR;
using MyModule.Application.Dtos;
using MyModule.Domain.Interfaces;

namespace MyModule.Application.Queries.GetMyAggregateById;

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
        return new MyAggregateDto(aggregate.Id, "Name from aggregate");
    }
}
