using System.Collections.Generic;
using MediatR;
using PineConePro.Erp.PIM.Application.Dtos;

namespace PineConePro.Erp.PIM.Application.Queries.GetAllMyAggregates;

/// <summary>
/// Represents the query to retrieve all aggregates.
/// </summary>
public sealed record GetAllMyAggregatesQuery : IRequest<IReadOnlyCollection<MyAggregateDto>>;
