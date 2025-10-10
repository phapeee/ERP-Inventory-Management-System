using MediatR;

namespace PineConePro.Erp.PIM.Application.Commands.CreateMyAggregate;

/// <summary>
/// Represents the command to create a new MyAggregate.
/// </summary>
/// <param name="Name">The name of the aggregate.</param>
public sealed record CreateMyAggregateCommand(string Name) : IRequest<Guid>;
