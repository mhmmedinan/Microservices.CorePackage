using Core.Abstractions.Messaging.Outbox;

namespace Core.Messaging.InMemory.Outbox;

public class InMemoryOutboxStore : IInMemoryOutboxStore
{
    public IList<OutboxMessage> Events { get; } = new List<OutboxMessage>();
}
