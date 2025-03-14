using MediatR;

namespace Core.Abstractions.Events;

/// <summary>
/// Represents the base interface for all events in the system.
/// This interface is used to define domain events that represent something significant that has happened in the domain.
/// Events are immutable and represent past occurrences.
/// </summary>
/// <remarks>
/// Use this interface when you need to:
/// <list type="bullet">
/// <item>Implement domain events for domain-driven design</item>
/// <item>Notify other parts of the system about changes</item>
/// <item>Maintain an audit trail of system changes</item>
/// <item>Implement event sourcing patterns</item>
/// </list>
/// </remarks>
public interface IEvent : INotification
{
    /// <summary>
    /// Gets the unique identifier for the event.
    /// Used to uniquely identify each event instance.
    /// </summary>
    Guid EventId { get; }

    /// <summary>
    /// Gets the version number of the event.
    /// Used for event versioning and handling event schema evolution.
    /// </summary>
    long EventVersion { get; }

    /// <summary>
    /// Gets the date and time when the event occurred.
    /// Represents the exact moment when the event was created.
    /// </summary>
    DateTime OccurredOn { get; }

    /// <summary>
    /// Gets the timestamp of when the event was created.
    /// Includes timezone information for distributed systems.
    /// </summary>
    DateTimeOffset TimeStamp { get; }

    /// <summary>
    /// Gets the type name of the event.
    /// Used for event type discrimination and serialization.
    /// </summary>
    string EventType { get; }
}
