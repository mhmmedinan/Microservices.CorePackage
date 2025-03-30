namespace Core.Abstractions.Messaging;

/// <summary>
/// Defines middleware for processing messages in the pipeline.
/// Middleware components provide a way to intercept and modify message processing behavior.
/// </summary>
/// <remarks>
/// Use this interface when you need to:
/// <list type="bullet">
/// <item>Implement cross-cutting concerns</item>
/// <item>Add logging or monitoring</item>
/// <item>Handle message validation</item>
/// <item>Implement error handling and retries</item>
/// <item>Manage transactions</item>
/// </list>
/// Common middleware scenarios:
/// <list type="bullet">
/// <item>Authentication and authorization</item>
/// <item>Message validation</item>
/// <item>Performance monitoring</item>
/// <item>Error handling</item>
/// <item>Caching</item>
/// </list>
/// </remarks>
public interface IMessageMiddleware<TMessage>
    where TMessage : IMessage
{
    /// <summary>
    /// Handles the message processing in the middleware pipeline.
    /// Executes middleware-specific logic and calls the next middleware in the chain.
    /// </summary>
    /// <typeparam name="TMessage">The type of message being processed.</typeparam>
    /// <param name="message">The message to process.</param>
    /// <param name="messageContext">The context containing metadata and state for message processing.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <param name="next">The delegate to the next middleware in the pipeline.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task HandleAsync(
        TMessage message,
        IMessageContext messageContext,
        CancellationToken cancellationToken,
        HandleMessageDelegate<TMessage> next);
}
