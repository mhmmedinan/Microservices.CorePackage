using Core.Abstractions.Events.External;

namespace Core.Abstractions.Messaging.Outbox;

// http://www.kamilgrzybek.com/design/the-outbox-pattern/
// https://event-driven.io/en/outbox_inbox_patterns_and_delivery_guarantees_explained/
// https://debezium.io/blog/2019/02/19/reliable-microservices-data-exchange-with-the-outbox-pattern/

public interface IOutboxService
{
    Task<IEnumerable<OutboxMessage>> GetAllUnsentOutboxMessagesAsync(
       EventType eventType = EventType.IntegrationEvent,
       CancellationToken cancellationToken = default);

    Task<IEnumerable<OutboxMessage>> GetAllOutboxMessagesAsync(
        EventType eventType = EventType.IntegrationEvent,
        CancellationToken cancellationToken = default);

    Task CleanProcessedAsync(CancellationToken cancellationToken = default);

    Task SaveAsync(IIntegrationEvent integrationEvent, CancellationToken cancellationToken = default);
    Task SaveAsync(IIntegrationEvent[] integrationEvents, CancellationToken cancellationToken = default);

    Task PublishUnsentOutboxMessagesAsync(CancellationToken cancellationToken = default);

}
