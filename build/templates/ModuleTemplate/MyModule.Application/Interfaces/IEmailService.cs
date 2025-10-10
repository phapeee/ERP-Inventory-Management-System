namespace PineConePro.Erp.MyModule.Application.Interfaces;

/// <summary>
/// Defines the email service.
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Sends an email.
    /// </summary>
    /// <param name="recipient">The recipient of the email.</param>
    /// <param name="subject">The subject of the email.</param>
    /// <param name="body">The body of the email.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task SendEmailAsync(string recipient, string subject, string body);
}
