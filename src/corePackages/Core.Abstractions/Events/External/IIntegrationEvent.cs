using Core.Abstractions.Events;

namespace Core.Abstractions.Events.External;

/// <summary>
/// Represents an integration event that can be published to external systems.
/// </summary>
public interface IIntegrationEvent : IEvent
{
    public string CorrelationId { get; }
}
