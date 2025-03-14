namespace Core.Abstractions.Messaging;

/// <summary>
/// Represents the base interface for all messages in the messaging system.
/// Messages are the fundamental units of communication between components in a distributed system.
/// </summary>
/// <remarks>
/// Use this interface when you need to:
/// <list type="bullet">
/// <item>Define messages for service-to-service communication</item>
/// <item>Implement message-based integration patterns</item>
/// <item>Create message-driven workflows</item>
/// <item>Handle asynchronous communication between components</item>
/// </list>
/// </remarks>
public interface IMessage
{
    /// <summary>
    /// Gets the unique identifier for the message.
    /// Used for message tracking, deduplication, and correlation.
    /// </summary>
    string MessageId { get; }

    /// <summary>
    /// Gets the correlation identifier for the message.
    /// Used to track related messages in a distributed transaction or workflow.
    /// </summary>
    string CorrelationId { get; }

    /// <summary>
    /// Gets the timestamp when the message was created.
    /// </summary>
    DateTime CreatedAt { get; }

    /// <summary>
    /// Gets the type of the message.
    /// Used for message routing and handling.
    /// </summary>
    string MessageType { get; }
}
