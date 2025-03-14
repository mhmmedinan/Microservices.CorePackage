using Microsoft.Extensions.Logging;

namespace Core.Abstractions.Events.External;


public abstract record IntegrationEvent : Event, IIntegrationEvent
{
    public string CorrelationId { get; protected set; } = default;
}

