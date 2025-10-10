namespace PineConePro.Erp.PIM.Domain.Events;

// Domain events are used to capture things that have happened in the domain.
// They are often used for side-effect-free communication between aggregates or to publish integration events.

/// <summary>
/// Represents the event that is raised when a new MyAggregate is created.
/// </summary>
/// <param name="AggregateId">The identifier of the aggregate.</param>
/// <param name="OccurredOn">The date and time when the event occurred.</param>
public sealed record MyAggregateCreatedEvent(Guid AggregateId, DateTime OccurredOn);
