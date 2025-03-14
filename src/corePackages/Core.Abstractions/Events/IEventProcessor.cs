namespace Core.Abstractions.Events;

/// <summary>
/// Defines the contract for processing events in the system.
/// The event processor is responsible for managing the lifecycle of events, including their publication and dispatch.
/// </summary>
/// <remarks>
/// Use this interface when you need to:
/// <list type="bullet">
/// <item>Implement event processing infrastructure</item>
/// <item>Handle event publication and distribution</item>
/// <item>Manage event delivery guarantees</item>
/// <item>Implement event routing logic</item>
/// </list>
/// </remarks>
public interface IEventProcessor
{
    /// <summary>
    /// Publishes a single event asynchronously.
    /// This method is used when you want to broadcast an event to all interested handlers.
    /// </summary>
    /// <typeparam name="TEvent">The type of event to publish.</typeparam>
    /// <param name="event">The event to publish.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IEvent;

    /// <summary>
    /// Publishes multiple events asynchronously.
    /// Useful for batch processing or when multiple events need to be published atomically.
    /// </summary>
    /// <typeparam name="TEvent">The type of events to publish.</typeparam>
    /// <param name="events">The array of events to publish.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    Task PublishAsync<TEvent>(TEvent[] events, CancellationToken cancellationToken = default)
        where TEvent : IEvent;

    /// <summary>
    /// Dispatches a single event asynchronously to its handlers.
    /// This method is used when you want to directly send an event to specific handlers.
    /// </summary>
    /// <typeparam name="TEvent">The type of event to dispatch.</typeparam>
    /// <param name="event">The event to dispatch.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    Task DispatchAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IEvent;

    /// <summary>
    /// Dispatches multiple events asynchronously to their respective handlers.
    /// Useful for batch processing or when multiple events need to be dispatched atomically.
    /// </summary>
    /// <typeparam name="TEvent">The type of events to dispatch.</typeparam>
    /// <param name="events">The array of events to dispatch.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    Task DispatchAsync<TEvent>(TEvent[] events, CancellationToken cancellationToken = default)
        where TEvent : IEvent;
}