namespace Core.Abstractions.Messaging;

public interface IMessage
{
    Guid Id { get; }
    Guid CorrelationId { get; }
    DateTime OccurredOn { get; }
    string MessageType { get; }
}
