using MediatR;
using PineConePro.Erp.Shipping.Application.Dtos;

namespace PineConePro.Erp.Shipping.Application.Queries.GetMyAggregateById;

/// <summary>
/// Represents the query to get an aggregate by its identifier.
/// </summary>
/// <param name="Id">The identifier of the aggregate.</param>
public sealed record GetMyAggregateByIdQuery(Guid Id) : IRequest<MyAggregateDto?>;