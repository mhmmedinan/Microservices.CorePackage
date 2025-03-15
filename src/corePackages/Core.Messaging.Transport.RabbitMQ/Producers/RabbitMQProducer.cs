using Core.Abstractions.Events.External;
using Core.Abstractions.Messaging.Serialization;
using Core.Abstractions.Messaging.Transport;
using Microsoft.Extensions.Logging;
using Polly;
using System.Text;

namespace Core.Messaging.Transport.RabbitMQ.Producers;

/// <summary>
/// Implements a RabbitMQ producer for publishing integration events.
/// </summary>
public class RabbitMQProducer : IEventBusPublisher
{
    private readonly IPublisherChannelFactory _publisherChannelFactory;
    private readonly IMessageSerializer _messageSerializer;
    private readonly ILogger<RabbitMQProducer> _logger;

    /// <summary>
    /// Initializes a new instance of the RabbitMQProducer class.
    /// </summary>
    /// <param name="messageSerializer">Serializer for message content.</param>
    /// <param name="logger">Logger for producer operations.</param>
    /// <param name="publisherChannelFactory">Factory for creating publisher channels.</param>
    /// <exception cref="ArgumentNullException">Thrown when any required dependency is null.</exception>
    public RabbitMQProducer(
        IMessageSerializer messageSerializer,
        ILogger<RabbitMQProducer> logger,
        IPublisherChannelFactory publisherChannelFactory)
    {
        _messageSerializer = messageSerializer ?? throw new ArgumentNullException(nameof(messageSerializer));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _publisherChannelFactory =
            publisherChannelFactory ?? throw new ArgumentNullException(nameof(publisherChannelFactory));
    }

    /// <summary>
    /// Publishes an integration event to RabbitMQ.
    /// </summary>
    /// <typeparam name="TEvent">The type of the integration event.</typeparam>
    /// <param name="integrationEvent">The integration event to publish.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when integrationEvent is null.</exception>
    public Task PublishAsync<TEvent>(TEvent integrationEvent, CancellationToken cancellationToken = default) where TEvent : IIntegrationEvent
    {
       if (integrationEvent == null)
            throw new ArgumentNullException(nameof(integrationEvent));

        using var context = _publisherChannelFactory.Create(integrationEvent);

        var encodedMessage = _messageSerializer.Serialize(integrationEvent);

        var properties = context.Channel.CreateBasicProperties();
        properties.Persistent = true;
        properties.Headers = new Dictionary<string, object>
        {
            {
                HeaderNames.MessageType,
                integrationEvent.GetType().Name
            }
        };

        var policy = Policy.Handle<System.Exception>().WaitAndRetry(3,retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
        {
            _logger.LogWarning(ex,
                  "Could not publish message '{MessageId}' to Exchange '{ExchangeName}', after {Timeout}s : {ExceptionMessage}",
                  integrationEvent.EventId,
                  context.QueueReferences.ExchangeName,
                 $"{time.TotalSeconds:n1}", ex.Message);
        });


        policy.Execute(() =>
        {
            context.Channel.BasicPublish(
                exchange: context.QueueReferences.ExchangeName,
                routingKey: context.QueueReferences.RoutingKey,
                mandatory: true,
                basicProperties: properties,
                body: Encoding.UTF8.GetBytes(encodedMessage));
            _logger.LogInformation("message '{MessageId}' published to Exchange '{ExchangeName}'",
                integrationEvent.EventId,
                context.QueueReferences.ExchangeName);
        });
        return Task.CompletedTask;
    }
}
