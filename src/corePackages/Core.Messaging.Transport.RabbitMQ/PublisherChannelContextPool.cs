using RabbitMQ.Client;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Core.Messaging.Transport.RabbitMQ;

/// <summary>
/// Implements a pool for managing RabbitMQ publisher channel contexts.
/// </summary>
public sealed class PublisherChannelContextPool : IPublisherChannelContextPool, IDisposable
{
    private readonly IBusConnection _connection;
    private readonly ILogger<PublisherChannelContext> _logger;
    private readonly ConcurrentDictionary<string, ConcurrentBag<PublisherChannelContext>> _pools = new();

    /// <summary>
    /// Initializes a new instance of the PublisherChannelContextPool class.
    /// </summary>
    /// <param name="connection">The RabbitMQ connection.</param>
    /// <param name="logger">Logger for channel context operations.</param>
    /// <exception cref="ArgumentNullException">Thrown when connection or logger is null.</exception>
    public PublisherChannelContextPool(IBusConnection connection, ILogger<PublisherChannelContext> logger)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets a channel context from the pool for the specified queue references.
    /// </summary>
    /// <param name="references">The queue references for the channel.</param>
    /// <returns>A publisher channel context.</returns>
    /// <exception cref="ArgumentNullException">Thrown when references is null.</exception>
    public async Task<PublisherChannelContext> GetAsync(QueueReferences references)
    {
        if (references == null)
            throw new ArgumentNullException(nameof(references));

        var pool = _pools.GetOrAdd(references.ExchangeName, _ => new());

        if (!pool.TryTake(out var ctx))
        {
            var channel = await _connection.CreateChannelAsync();
            await channel.ExchangeDeclareAsync(exchange: references.ExchangeName, type: ExchangeType.Topic);

            ctx = new PublisherChannelContext(channel, references, this, _logger);
            _logger.LogDebug("Created new channel context for exchange '{ExchangeName}'", references.ExchangeName);
        }
        else
        {
            _logger.LogDebug("Retrieved channel context from pool for exchange '{ExchangeName}'", references.ExchangeName);
        }

        return ctx;
    }


    /// <summary>
    /// Returns a channel context to the pool.
    /// </summary>
    /// <param name="ctx">The channel context to return.</param>
    /// <exception cref="ArgumentNullException">Thrown when ctx is null.</exception>
    public void Return(PublisherChannelContext ctx)
    {
        if (ctx == null)
            throw new ArgumentNullException(nameof(ctx));

        if (ctx.Channel.IsClosed)
            return;

        var pool = _pools.GetOrAdd(ctx.QueueReferences.ExchangeName, _ => new());
        pool.Add(ctx);
    }

    /// <summary>
    /// Gets the number of available channel contexts in the pool.
    /// </summary>
    /// <returns>The total count of available channel contexts.</returns>
    public int GetAvailableCount() => _pools.Sum(p => p.Value.Count);

    /// <summary>
    /// Disposes all channel contexts in the pool and clears the pool.
    /// </summary>
    public void Dispose()
    {
        foreach (var pool in _pools.Values)
        {
            foreach (var ctx in pool)
            {
                if (ctx.Channel.IsOpen)
                    ctx.Channel.CloseAsync();
                ctx.Channel.Dispose();
            }
        }

        _pools.Clear();
    }
}
