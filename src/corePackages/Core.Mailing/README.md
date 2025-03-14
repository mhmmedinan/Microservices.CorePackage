# Core.Mailing Package

This package provides a flexible and easy-to-use email sending functionality for .NET applications. It offers a clean abstraction for sending emails with support for various email providers through MailKit implementation.

## Features

- Simple and intuitive API for sending emails
- Support for:
  - HTML and plain text email content
  - File attachments
  - Multiple recipients
  - Custom sender information
- Configurable mail settings
- Async operations
- MailKit integration
- SSL/TLS support

## Installation

```bash
dotnet add package Core.Mailing
```

## Configuration

Add the following configuration to your `appsettings.json`:

```json
{
  "MailSettings": {
    "Server": "smtp.example.com",
    "Port": 587,
    "SenderEmail": "noreply@example.com",
    "SenderFullName": "Your Application Name",
    "UserName": "your-username",
    "Password": "your-password",
    "SSL": true,
    "HTML": true
  }
}
```

Register the mailing service in your `Startup.cs` or `Program.cs`:

```csharp
services.Configure<MailSettings>(configuration.GetSection("MailSettings"));
services.AddSingleton<IMailService, MailKitMailService>();
```

## Usage Examples

### Basic Email Sending

```csharp
var mail = new Mail
{
    Subject = "Welcome to Our Application",
    TextBody = "Welcome! Thank you for joining us.",
    HtmlBody = "<h1>Welcome!</h1><p>Thank you for joining us.</p>",
    ToList = new List<ToEmail>
    {
        new ToEmail("user@example.com", "John Doe")
    }
};

await _mailService.SendEmailAsync(mail);
```

### Sending with Attachments

```csharp
var attachment = new MimePart("application", "pdf")
{
    Content = new MimeContent(File.OpenRead("document.pdf")),
    ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
    ContentTransferEncoding = ContentEncoding.Base64,
    FileName = "document.pdf"
};

var mail = new Mail
{
    Subject = "Document Attached",
    TextBody = "Please find the attached document.",
    HtmlBody = "<p>Please find the attached document.</p>",
    ToList = new List<ToEmail>
    {
        new ToEmail("user@example.com", "John Doe")
    },
    Attachments = new List<MimeEntity> { attachment }
};

await _mailService.SendEmailAsync(mail);
```

### Multiple Recipients

```csharp
var mail = new Mail
{
    Subject = "Team Meeting",
    TextBody = "Team meeting scheduled for tomorrow.",
    HtmlBody = "<p>Team meeting scheduled for tomorrow.</p>",
    ToList = new List<ToEmail>
    {
        new ToEmail("team1@example.com", "Team Member 1"),
        new ToEmail("team2@example.com", "Team Member 2"),
        new ToEmail("team3@example.com", "Team Member 3")
    }
};

await _mailService.SendEmailAsync(mail);
```

## Best Practices

1. **Configuration Security**
   - Never store email credentials in code
   - Use secure configuration management
   - Consider using secret management services in production

2. **Error Handling**
   - Always handle email sending exceptions
   - Implement retry logic for transient failures
   - Log email sending results

3. **Performance**
   - Use async methods for sending emails
   - Consider implementing email queuing for bulk sending
   - Monitor email sending performance

4. **Content**
   - Always provide both HTML and plain text versions
   - Follow email best practices for content
   - Test emails in different email clients

## Contributing

Please read our contributing guidelines and code of conduct before making a pull request.

## License

This project is licensed under the MIT License - see the LICENSE file for details. 