using MediatR;

namespace Core.Abstractions.Events;

/// <summary>
/// Defines a handler for processing events in the system.
/// Event handlers contain the business logic that should be executed when specific events occur.
/// </summary>
/// <typeparam name="TEvent">The type of event being handled.</typeparam>
/// <remarks>
/// Use this interface when you need to:
/// <list type="bullet">
/// <item>React to domain events in the system</item>
/// <item>Implement side effects of business operations</item>
/// <item>Update read models in CQRS architecture</item>
/// <item>Integrate with external systems based on events</item>
/// <item>Implement event-driven workflows</item>
/// </list>
/// </remarks>
public interface IEventHandler<in TEvent> : INotificationHandler<TEvent>
where TEvent : IEvent
{
}
