using MediatR;
using PineConePro.Erp.OrderRMA.Application.Dtos;

namespace PineConePro.Erp.OrderRMA.Application.Queries.GetMyAggregateById;

/// <summary>
/// Represents the query to get an aggregate by its identifier.
/// </summary>
/// <param name="Id">The identifier of the aggregate.</param>
public sealed record GetMyAggregateByIdQuery(Guid Id) : IRequest<MyAggregateDto?>;