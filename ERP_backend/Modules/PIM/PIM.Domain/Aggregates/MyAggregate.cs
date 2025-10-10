using System;

namespace PIM.Domain.Aggregates;

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
    public Guid Id { get; }

    /// <summary>
    /// Gets the aggregate name.
    /// </summary>
    public string Name { get; private set; }

    // Private constructor to enforce creation via factory or specific methods
    private MyAggregate(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    /// <summary>
    /// Creates a new instance of the <see cref="MyAggregate"/> class.
    /// </summary>
    /// <param name="name">The aggregate name.</param>
    /// <returns>The new aggregate.</returns>
    public static MyAggregate Create(string name)
    {
        var validatedName = ValidateName(name);

        var aggregate = new MyAggregate(Guid.NewGuid(), validatedName);

        // TODO: Raise a domain event, e.g., MyAggregateCreatedEvent

        return aggregate;
    }

    /// <summary>
    /// Updates the aggregate name.
    /// </summary>
    /// <param name="name">The new aggregate name.</param>
    public void UpdateName(string name)
    {
        Name = ValidateName(name);
        // TODO: Raise a domain event, e.g., MyAggregateUpdatedEvent
    }

    private static string ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name cannot be null or whitespace.", nameof(name));
        }

        return name.Trim();
    }
}
