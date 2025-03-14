using Core.Abstractions.Messaging.Outbox;

namespace Core.Messaging.InMemory.Outbox;

/// <summary>
/// Implementation of in-memory outbox message storage
/// </summary>
public class InMemoryOutboxStore : IInMemoryOutboxStore
{
    /// <summary>
    /// Gets the list of outbox messages stored in memory
    /// </summary>
    public IList<OutboxMessage> Events { get; } = new List<OutboxMessage>();
}
