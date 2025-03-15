using RabbitMQ.Client;

namespace Core.Messaging.Transport.RabbitMQ;

/// <summary>
/// Defines the contract for a RabbitMQ bus connection.
/// </summary>
public interface IBusConnection
{
    /// <summary>
    /// Gets a value indicating whether the connection is currently established and open.
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// Creates a new channel for communication with RabbitMQ.
    /// </summary>
    /// <returns>A new channel instance.</returns>
    IModel CreateChannel();
}
