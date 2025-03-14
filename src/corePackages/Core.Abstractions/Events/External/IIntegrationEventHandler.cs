using Core.Abstractions.Events;
using Core.Abstractions.Events.External;

/// <summary>
/// Defines a handler for processing integration events from external systems.
/// Integration event handlers contain the logic for processing events that cross service boundaries.
/// </summary>
/// <typeparam name="TIntegrationEvent">The type of integration event being handled.</typeparam>
/// <remarks>
/// Use this interface when you need to:
/// <list type="bullet">
/// <item>Handle events from other microservices</item>
/// <item>Implement cross-service business processes</item>
/// <item>Maintain data consistency across services</item>
/// <item>React to external system state changes</item>
/// </list>
/// </remarks>
public interface IIntegrationEventHandler<in TIntegrationEvent> : IEventHandler<TIntegrationEvent>
    where TIntegrationEvent : IIntegrationEvent
{
}