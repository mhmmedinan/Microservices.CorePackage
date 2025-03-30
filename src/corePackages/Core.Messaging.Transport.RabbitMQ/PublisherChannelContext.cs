using RabbitMQ.Client;
using Microsoft.Extensions.Logging;

namespace Core.Messaging.Transport.RabbitMQ;

/// <summary>
/// Represents a context for a RabbitMQ publisher channel.
/// </summary>
public class PublisherChannelContext : IDisposable
{
    private readonly IPublisherChannelContextPool _publisherChannelContextPool;
    private readonly ILogger<PublisherChannelContext> _logger;

    /// <summary>
    /// Initializes a new instance of the PublisherChannelContext class.
    /// </summary>
    /// <param name="channel">The RabbitMQ channel.</param>
    /// <param name="queueReferences">The queue references for the channel.</param>
    /// <param name="publisherChannelContextPool">The pool that manages this context.</param>
    /// <param name="logger">Logger for context operations.</param>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is null.</exception>
    public PublisherChannelContext(
        IChannel channel,
        QueueReferences queueReferences,
        IPublisherChannelContextPool publisherChannelContextPool,
        ILogger<PublisherChannelContext> logger)
    {
        Channel = channel ?? throw new ArgumentNullException(nameof(channel));
        QueueReferences = queueReferences ?? throw new ArgumentNullException(nameof(queueReferences));
        _publisherChannelContextPool = publisherChannelContextPool ?? throw new ArgumentNullException(nameof(publisherChannelContextPool));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets the RabbitMQ channel.
    /// </summary>
    public IChannel Channel { get; }

    /// <summary>
    /// Gets the queue references for this context.
    /// </summary>
    public QueueReferences QueueReferences { get; }

    /// <summary>
    /// Returns the channel context to the pool.
    /// </summary>
    public void Dispose()
    {
        try
        {
            if (Channel.IsOpen)
            {
                _publisherChannelContextPool.Return(this);
                _logger.LogDebug("Channel context returned to pool for exchange '{ExchangeName}'", QueueReferences.ExchangeName);
            }
            else
            {
                _logger.LogWarning("Attempted to return closed channel to pool for exchange '{ExchangeName}'", QueueReferences.ExchangeName);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while returning channel context to pool for exchange '{ExchangeName}'", QueueReferences.ExchangeName);
            throw; // Re-throw to ensure proper cleanup
        }
    }
}
