using System;
using MediatR;

namespace PineConePro.Erp.PIM.Application.Commands.DeleteMyAggregate;

/// <summary>
/// Represents the command to delete an aggregate.
/// </summary>
/// <param name="Id">The identifier of the aggregate.</param>
public sealed record DeleteMyAggregateCommand(Guid Id) : IRequest<bool>;
