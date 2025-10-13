using System.Collections.Generic;
using MediatR;
using PineConePro.Erp.Shipping.Application.Dtos;

namespace PineConePro.Erp.Shipping.Application.Queries.GetAllMyAggregates;

/// <summary>
/// Represents the query to retrieve all aggregates.
/// </summary>
public sealed record GetAllMyAggregatesQuery : IRequest<IReadOnlyCollection<MyAggregateDto>>;
