using Core.Abstractions.Events.External;
using Core.Abstractions.Events;
using Core.Abstractions.Messaging.Transport;
using Core.Abstractions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Reflection;
using IModel = RabbitMQ.Client.IModel;

namespace Core.Messaging.Transport.RabbitMQ.Consumers;

/// <summary>
/// Implements a RabbitMQ consumer for handling integration events.
/// </summary>
public class RabbitMQConsumer : IEventBusSubscriber
{
    private readonly IBusConnection _connection;
    private readonly IMessageParser _messageParser;
    private readonly ILogger<RabbitMQConsumer> _logger; 
    private readonly RabbitConfiguration _rabbitConfiguration;
    private readonly IServiceProvider _serviceProvider;
    private readonly List<IModel> _channels = new();

    /// <summary>
    /// Initializes a new instance of the RabbitMQConsumer class.
    /// </summary>
    /// <param name="connection">The RabbitMQ connection.</param>
    /// <param name="messageParser">Parser for handling message serialization.</param>
    /// <param name="logger">Logger for consumer operations.</param>
    /// <param name="rabbitConfiguration">RabbitMQ configuration settings.</param>
    /// <param name="serviceProvider">Service provider for dependency injection.</param>
    /// <exception cref="ArgumentNullException">Thrown when any required dependency is null.</exception>
    public RabbitMQConsumer(
        IBusConnection connection,
        IMessageParser messageParser,
        ILogger<RabbitMQConsumer> logger,
        RabbitConfiguration rabbitConfiguration,
        IServiceProvider serviceProvider)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _messageParser = messageParser ?? throw new ArgumentNullException(nameof(messageParser));
        _rabbitConfiguration = rabbitConfiguration ?? throw new ArgumentNullException(nameof(rabbitConfiguration));
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Starts the consumer and initializes message subscriptions.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for stopping the consumer.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task StartAsync(CancellationToken cancellationToken = default)
    {
        var messageTypes = AppDomain.CurrentDomain.GetAssemblies().GetHandledIntegrationEventTypes();
        var factory = _serviceProvider.GetRequiredService<IQueueReferenceFactory>();

        foreach(var messageType in messageTypes)
        {
            // https://www.davidguida.net/dynamic-method-invocation-with-net-core/
            // https://www.thereformedprogrammer.net/using-net-generics-with-a-type-derived-at-runtime/
            MethodInfo methodInfo = typeof(IQueueReferenceFactory).GetMethod("Create");
            MethodInfo generic = methodInfo.MakeGenericMethod(messageType);

            var queueReferences = generic.Invoke(factory, new object[] { null }) as QueueReferences;
            var channel = InitChannel(queueReferences);
            _channels.Add(channel);
            InitSubscription(queueReferences, channel);
        }
        return Task.CompletedTask;
    }

    /// <summary>
    /// Stops the consumer and closes all channels.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for stopping the consumer.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task StopAsync(CancellationToken cancellationToken = default)
    {
        _channels.ForEach(StopChannel);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Initializes a subscription for a specific queue.
    /// </summary>
    /// <param name="queueReferences">Queue reference information.</param>
    /// <param name="channel">The RabbitMQ channel.</param>
    private void InitSubscription(QueueReferences queueReferences, IModel channel)
    {
        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.Received += OnMessageReceivedAsync;

        _logger.LogInformation($"initializing subscription on queue '{queueReferences.QueueName}' ...");
        channel.BasicConsume(queue: queueReferences.QueueName, autoAck: false, consumer: consumer);
    }

    /// <summary>
    /// Initializes a channel with the specified queue configuration.
    /// </summary>
    /// <param name="queueReferences">Queue reference information.</param>
    /// <returns>The initialized RabbitMQ channel.</returns>
    private IModel InitChannel(QueueReferences queueReferences)
    {
        var channel = _connection.CreateChannel();

        _logger.LogInformation(
            $"initializing queue '{queueReferences.QueueName}' on exchange '{queueReferences.ExchangeName}'...");

        channel.ExchangeDeclare(exchange: queueReferences.ExchangeName, type: ExchangeType.Topic);
        channel.QueueDeclare(queue: queueReferences.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: new Dictionary<string, object>()
            {
                { Headers.XDeadLetterExchange, queueReferences.DeadLetterExchangeName },
                { Headers.XDeadLetterRoutingKey, queueReferences.DeadLetterQueue }
            });
        channel.QueueBind(queue: queueReferences.QueueName,
            exchange: queueReferences.ExchangeName,
            routingKey: queueReferences.RoutingKey,
            arguments: null);

        channel.CallbackException += OnChannelException;

        return channel;
    }

    /// <summary>
    /// Handles channel exceptions.
    /// </summary>
    /// <param name="_">Sender object (unused).</param>
    /// <param name="ea">Exception event arguments.</param>
    private void OnChannelException(object _, CallbackExceptionEventArgs ea)
    {
        _logger.LogError(ea.Exception, "the RabbitMQ Channel has encountered an error: {ExceptionMessage}",
            ea.Exception.Message);

        // TODO --> Provide argument
        // InitChannel();
        // InitSubscription();
    }

    /// <summary>
    /// Handles received messages asynchronously.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="eventArgs">Event arguments containing the message data.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task OnMessageReceivedAsync(object sender, BasicDeliverEventArgs eventArgs)
    {
        var consumer = sender as IBasicConsumer;
        var channel = consumer.Model;

        IIntegrationEvent message;
        try
        {
            message = _messageParser.Resolve(eventArgs.BasicProperties, eventArgs.Body.ToArray());
        }
        catch (System.Exception ex)
        {
            _logger.LogError(
                ex,
                "an exception has occured while decoding queue message from Exchange '{ExchangeName}', message cannot be parsed. Error: {ExceptionMessage}",
                eventArgs.Exchange,
                ex.Message);
            channel.BasicReject(eventArgs.DeliveryTag, requeue: false);

            return;
        }

        _logger.LogInformation(
            "received message '{MessageId}' from Exchange '{ExchangeName}''. Processing...",
            eventArgs.BasicProperties.MessageId,
            eventArgs.Exchange);
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var eventProcessor = scope.ServiceProvider.GetRequiredService<IEventProcessor>();

            // Publish to internal event bus
            await eventProcessor.DispatchAsync(message, default).ConfigureAwait(false);

            channel.BasicAck(eventArgs.DeliveryTag, multiple: false);
        }
        catch (System.Exception ex)
        {
            HandleConsumerException(ex, eventArgs, channel, message);
        }
    }

    /// <summary>
    /// Handles consumer exceptions during message processing.
    /// </summary>
    /// <param name="ex">The exception that occurred.</param>
    /// <param name="deliveryProps">Delivery properties of the message.</param>
    /// <param name="channel">The RabbitMQ channel.</param>
    /// <param name="message">The integration event being processed.</param>
    private void HandleConsumerException(
       System.Exception ex,
       BasicDeliverEventArgs deliveryProps,
       IModel channel,
       IIntegrationEvent message)
    {
        _logger.LogWarning(
            "an error has occurred while processing Message '{MessageId}' from Exchange '{ExchangeName}' : {ExceptionMessage}",
            message.EventId,
            deliveryProps.Exchange,
            ex.Message);

        channel.BasicReject(deliveryProps.DeliveryTag, requeue: false);
    }

    /// <summary>
    /// Stops and disposes a channel.
    /// </summary>
    /// <param name="channel">The channel to stop.</param>
    private void StopChannel(IModel channel)
    {
        if (channel is null)
            return;

        channel.CallbackException -= OnChannelException;

        if (channel.IsOpen)
            channel.Close();

        channel.Dispose();
        channel = null;
    }

    /// <summary>
    /// Disposes the consumer and releases all resources.
    /// </summary>
    public void Dispose()
    {
        _channels.ForEach(StopChannel);
    }
}
