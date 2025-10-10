using System.Collections.Generic;
using MediatR;
using MyModule.Application.Dtos;

namespace MyModule.Application.Queries.GetAllMyAggregates;

/// <summary>
/// Represents the query to retrieve all aggregates.
/// </summary>
public sealed record GetAllMyAggregatesQuery : IRequest<IReadOnlyCollection<MyAggregateDto>>;
