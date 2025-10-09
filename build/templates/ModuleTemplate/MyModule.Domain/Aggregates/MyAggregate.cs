namespace MyModule.Domain.Aggregates;

// An aggregate root is a specific type of entity that serves as the entry point for an aggregate.
// Aggregates are clusters of domain objects that can be treated as a single unit.

/// <summary>
/// Represents the MyAggregate aggregate root.
/// </summary>
public sealed class MyAggregate
{
    /// <summary>
    /// Gets the aggregate identifier.
    /// </summary>
    public Guid Id { get; private set; }

    // Private constructor to enforce creation via factory or specific methods
    private MyAggregate(Guid id)
    {
        Id = id;
    }

    /// <summary>
    /// Creates a new instance of the <see cref="MyAggregate"/> class.
    /// </summary>
    /// <returns>The new aggregate.</returns>
    public static MyAggregate Create()
    {
        var aggregate = new MyAggregate(Guid.NewGuid());

        // TODO: Raise a domain event, e.g., MyAggregateCreatedEvent

        return aggregate;
    }
}
