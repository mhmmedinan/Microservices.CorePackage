namespace Core.Messaging.Transport.RabbitMQ;

/// <summary>
/// Defines the contract for managing a pool of publisher channel contexts.
/// </summary>
public interface IPublisherChannelContextPool
{
    /// <summary>
    /// Gets a channel context from the pool for the specified queue references.
    /// </summary>
    /// <param name="references">The queue references for the channel.</param>
    /// <returns>A publisher channel context.</returns>
    Task<PublisherChannelContext> GetAsync(QueueReferences references);

    /// <summary>
    /// Returns a channel context to the pool.
    /// </summary>
    /// <param name="ctx">The channel context to return.</param>
    void Return(PublisherChannelContext ctx);
}
