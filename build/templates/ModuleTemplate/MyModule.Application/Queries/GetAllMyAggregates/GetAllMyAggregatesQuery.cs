using System.Collections.Generic;
using MediatR;
using PineConePro.Erp.MyModule.Application.Dtos;

namespace PineConePro.Erp.MyModule.Application.Queries.GetAllMyAggregates;

/// <summary>
/// Represents the query to retrieve all aggregates.
/// </summary>
public sealed record GetAllMyAggregatesQuery : IRequest<IReadOnlyCollection<MyAggregateDto>>;
