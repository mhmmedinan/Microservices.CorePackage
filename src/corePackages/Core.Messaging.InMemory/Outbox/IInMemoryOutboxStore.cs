using Core.Abstractions.Messaging.Outbox;

namespace Core.Messaging.InMemory.Outbox;

/// <summary>
/// Interface for in-memory outbox message storage
/// </summary>
public interface IInMemoryOutboxStore
{
    /// <summary>
    /// Gets the list of outbox messages stored in memory
    /// </summary>
    public IList<OutboxMessage> Events { get; }
}
