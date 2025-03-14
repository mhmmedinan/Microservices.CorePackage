namespace Core.Abstractions.Messaging;

/// <summary>
/// Defines the contract for dispatching messages to their handlers.
/// The message dispatcher is responsible for routing messages to appropriate handlers and managing the dispatch process.
/// </summary>
/// <remarks>
/// Use this interface when you need to:
/// <list type="bullet">
/// <item>Route messages to their handlers</item>
/// <item>Implement publish-subscribe patterns</item>
/// <item>Manage message dispatch lifecycle</item>
/// <item>Handle message routing and delivery</item>
/// </list>
/// Key responsibilities:
/// <list type="bullet">
/// <item>Handler resolution</item>
/// <item>Message routing</item>
/// <item>Dispatch coordination</item>
/// <item>Error handling</item>
/// </list>
/// </remarks>
public interface IMessageDispatcher
{
    /// <summary>
    /// Dispatches a message to its handlers asynchronously.
    /// Resolves the appropriate handlers and coordinates the message handling process.
    /// </summary>
    /// <typeparam name="TMessage">The type of message to dispatch.</typeparam>
    /// <param name="message">The message to dispatch.</param>
    /// <param name="messageContext">The context containing metadata and state for message processing.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DispatchAsync<TMessage>(
        TMessage message,
        IMessageContext messageContext,
        CancellationToken cancellationToken = default)
        where TMessage : IMessage;

    /// <summary>
    /// Dispatches multiple messages to their handlers asynchronously.
    /// Useful for batch processing or when multiple messages need to be dispatched atomically.
    /// </summary>
    /// <typeparam name="TMessage">The type of messages to dispatch.</typeparam>
    /// <param name="messages">The array of messages to dispatch.</param>
    /// <param name="messageContext">The context containing metadata and state for message processing.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DispatchAsync<TMessage>(
        TMessage[] messages,
        IMessageContext messageContext,
        CancellationToken cancellationToken = default)
        where TMessage : IMessage;
}
