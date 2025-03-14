namespace Core.Abstractions.Events.External;

public interface IIntegrationEvent : IEvent
{
    public string CorrelationId { get; }
}
