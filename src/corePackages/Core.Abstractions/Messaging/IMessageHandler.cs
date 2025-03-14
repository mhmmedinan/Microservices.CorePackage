namespace Core.Abstractions.Messaging;

/// <summary>
/// Defines a handler for processing messages in the messaging system.
/// Message handlers contain the business logic for processing specific types of messages.
/// </summary>
/// <typeparam name="TMessage">The type of message being handled.</typeparam>
/// <remarks>
/// Use this interface when you need to:
/// <list type="bullet">
/// <item>Process specific types of messages</item>
/// <item>Implement message-specific business logic</item>
/// <item>Handle message-based workflows</item>
/// <item>Implement message consumers</item>
/// </list>
/// Implementation considerations:
/// <list type="bullet">
/// <item>Handlers should be idempotent</item>
/// <item>Handlers should handle exceptions gracefully</item>
/// <item>Handlers should be stateless when possible</item>
/// </list>
/// </remarks>
public interface IMessageHandler<in TMessage> where TMessage : IMessage
{
    /// <summary>
    /// Handles the specified message asynchronously.
    /// Implements the business logic for processing the message.
    /// </summary>
    /// <param name="message">The message to handle.</param>
    /// <param name="messageContext">The context containing metadata and state for message processing.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task HandleAsync(
        TMessage message,
        IMessageContext messageContext,
        CancellationToken cancellationToken = default);
}
