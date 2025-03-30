namespace Core.Messaging.Transport.RabbitMQ;

/// <summary>
/// Configuration settings for RabbitMQ connection and behavior.
/// </summary>
public class RabbitConfiguration
{
    /// <summary>
    /// Gets or sets the hostname of the RabbitMQ server.
    /// </summary>
    public string HostName { get; set; }

    /// <summary>
    /// Gets or sets the port number of the RabbitMQ server.
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// Gets or sets the username for RabbitMQ authentication.
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// Gets or sets the password for RabbitMQ authentication.
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// Gets or sets the delay between retry attempts when connecting to RabbitMQ.
    /// </summary>
    public TimeSpan RetryDelay { get; set; }


}
