namespace Core.Abstractions.Events.External;

/// <summary>
/// Defines the contract for publishing integration events to external systems.
/// Handles the publication of events that need to be communicated across service boundaries.
/// </summary>
/// <remarks>
/// Use this interface when you need to:
/// <list type="bullet">
/// <item>Publish events to message brokers</item>
/// <item>Implement reliable message delivery</item>
/// <item>Handle cross-service event publication</item>
/// <item>Manage event serialization and transport</item>
/// </list>
/// </remarks>
public interface IIntegrationEventPublisher
{
    /// <summary>
    /// Publishes an integration event to external systems asynchronously.
    /// Handles the serialization and delivery of the event to the configured message broker.
    /// </summary>
    /// <param name="event">The integration event to publish.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    Task PublishAsync(IIntegrationEvent @event, CancellationToken cancellationToken = default);

    Task PublishAsync(IIntegrationEvent[] integrationEvents, CancellationToken cancellationToken = default);
}

