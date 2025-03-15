using Core.Abstractions.Events.External;

namespace Core.Messaging.Transport.RabbitMQ;

/// <summary>
/// Defines the contract for creating queue references for RabbitMQ messaging.
/// </summary>
public interface IQueueReferenceFactory
{
    /// <summary>
    /// Creates queue references for a specific message type.
    /// </summary>
    /// <typeparam name="TM">The type of the integration event.</typeparam>
    /// <param name="message">Optional message instance.</param>
    /// <returns>Queue references for the message type.</returns>
    QueueReferences Create<TM>(TM message = default)
        where TM : IIntegrationEvent;
}
