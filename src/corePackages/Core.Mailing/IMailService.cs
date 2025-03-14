namespace Core.Mailing;

/// <summary>
/// Defines the contract for email sending operations.
/// </summary>
public interface IMailService
{
    /// <summary>
    /// Sends an email asynchronously.
    /// </summary>
    /// <param name="mail">The mail object containing all the email details.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SendEmailAsync(Mail mail);
}
