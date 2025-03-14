namespace Core.Mailing;

/// <summary>
/// Represents an email recipient with their email address and full name.
/// </summary>
public class ToEmail
{
    /// <summary>
    /// Gets or sets the email address of the recipient.
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Gets or sets the full name of the recipient.
    /// </summary>
    public string FullName { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ToEmail"/> class.
    /// </summary>
    public ToEmail()
    {
        Email = string.Empty;
        FullName = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ToEmail"/> class with specified email and full name.
    /// </summary>
    /// <param name="email">The email address of the recipient.</param>
    /// <param name="fullName">The full name of the recipient.</param>
    public ToEmail(string email, string fullName)
    {
        Email = email;
        FullName = fullName;
    }
}
