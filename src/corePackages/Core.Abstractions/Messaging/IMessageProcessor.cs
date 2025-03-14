namespace Core.Abstractions.Messaging;

/// <summary>
/// Defines the contract for processing messages in the messaging system.
/// The message processor orchestrates the message handling pipeline, including middleware execution.
/// </summary>
/// <remarks>
/// Use this interface when you need to:
/// <list type="bullet">
/// <item>Implement message processing pipelines</item>
/// <item>Coordinate message handling workflows</item>
/// <item>Apply cross-cutting concerns to message processing</item>
/// <item>Manage message handling lifecycle</item>
/// </list>
/// Key responsibilities:
/// <list type="bullet">
/// <item>Message validation</item>
/// <item>Error handling and retries</item>
/// <item>Middleware coordination</item>
/// <item>Transaction management</item>
/// </list>
/// </remarks>
public interface IMessageProcessor
{
    /// <summary>
    /// Processes a message asynchronously through the handling pipeline.
    /// Coordinates the execution of middleware and the final message handler.
    /// </summary>
    /// <typeparam name="TMessage">The type of message to process.</typeparam>
    /// <param name="message">The message to process.</param>
    /// <param name="messageContext">The context containing metadata and state for message processing.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ProcessAsync<TMessage>(
        TMessage message,
        IMessageContext messageContext,
        CancellationToken cancellationToken = default)
        where TMessage : IMessage;
}