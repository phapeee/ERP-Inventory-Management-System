using System.Collections.Generic;
using MediatR;
using PIM.Application.Dtos;

namespace PIM.Application.Queries.GetAllMyAggregates;

/// <summary>
/// Represents the query to retrieve all aggregates.
/// </summary>
public sealed record GetAllMyAggregatesQuery : IRequest<IReadOnlyCollection<MyAggregateDto>>;
