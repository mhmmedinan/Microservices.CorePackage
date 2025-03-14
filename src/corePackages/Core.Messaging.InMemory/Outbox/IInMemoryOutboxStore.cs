using Core.Abstractions.Messaging.Outbox;

namespace Core.Messaging.InMemory.Outbox;

public interface IInMemoryOutboxStore
{
    public IList<OutboxMessage> Events { get; }
}
