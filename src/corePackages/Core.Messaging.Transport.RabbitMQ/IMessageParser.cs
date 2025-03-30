using Core.Abstractions.Events.External;
using RabbitMQ.Client;

namespace Core.Messaging.Transport.RabbitMQ;

/// <summary>
/// Defines the contract for parsing RabbitMQ messages into integration events.
/// </summary>
public interface IMessageParser
{
    /// <summary>
    /// Resolves a RabbitMQ message into an integration event.
    /// </summary>
    /// <param name="basicProperties">The message properties.</param>
    /// <param name="body">The message body as a byte array.</param>
    /// <returns>The resolved integration event.</returns>
    IIntegrationEvent Resolve(IReadOnlyBasicProperties basicProperties, byte[] body);
}
