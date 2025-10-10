namespace PineConePro.Erp.MyModule.Domain.Exceptions;

// Custom exceptions provide context-specific error information and can be used to handle specific failure scenarios.

/// <summary>
/// Represents errors that occur during domain processing.
/// </summary>
public class MyDomainException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MyDomainException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public MyDomainException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MyDomainException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public MyDomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
