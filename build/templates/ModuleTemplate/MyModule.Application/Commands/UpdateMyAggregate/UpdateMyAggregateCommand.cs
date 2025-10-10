using System;
using MediatR;

namespace MyModule.Application.Commands.UpdateMyAggregate;

/// <summary>
/// Represents the command to update an existing aggregate.
/// </summary>
/// <param name="Id">The identifier of the aggregate.</param>
/// <param name="Name">The new name of the aggregate.</param>
public sealed record UpdateMyAggregateCommand(Guid Id, string Name) : IRequest<bool>;
