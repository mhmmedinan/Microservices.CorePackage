namespace Core.Abstractions.Messaging;

/// <summary>
/// Represents a message that requires transactional processing.
/// </summary>
public interface ITransactionalMessage : IMessage
{
}
