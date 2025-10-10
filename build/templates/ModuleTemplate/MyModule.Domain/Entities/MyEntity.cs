namespace PineConePro.Erp.MyModule.Domain.Entities;

// Entities are objects that have a distinct identity that runs through time and different states.

/// <summary>
/// Represents the MyEntity entity.
/// </summary>
public sealed class MyEntity
{
    /// <summary>
    /// Gets the entity identifier.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets the entity name.
    /// </summary>
    public string? Name { get; private set; }

    // Private constructor for ORM or factory creation
    private MyEntity(Guid id, string? name)
    {
        Id = id;
        Name = name;
    }

    /// <summary>
    /// Creates a new instance of the <see cref="MyEntity"/> class.
    /// </summary>
    /// <param name="name">The name of the entity.</param>
    /// <returns>The new entity.</returns>
    public static MyEntity Create(string name)
    {
        // TODO: Add validation logic
        return new MyEntity(Guid.NewGuid(), name);
    }
}
