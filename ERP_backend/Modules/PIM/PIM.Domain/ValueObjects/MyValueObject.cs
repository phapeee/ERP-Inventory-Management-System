namespace PIM.Domain.ValueObjects;

// Value Objects are objects that we care about for what they are, not who or which they are.
// They are defined by their attributes and are typically immutable.

/// <summary>
/// Represents a value object.
/// </summary>
public class MyValueObject
{
    /// <summary>
    /// Gets the value.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MyValueObject"/> class.
    /// </summary>
    /// <param name="value">The value.</param>
    public MyValueObject(string value)
    {
        // TODO: Add validation
        Value = value;
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != GetType())
        {
            return false;
        }

        var other = (MyValueObject)obj;
        return Value == other.Value;
    }

    /// <summary>
    /// Serves as the default hash function.
    /// </summary>
    /// <returns>A hash code for the current object.</returns>
    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}
