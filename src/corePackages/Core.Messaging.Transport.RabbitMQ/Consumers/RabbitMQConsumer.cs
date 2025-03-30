using Core.Abstractions.Events.External;
using Core.Abstractions.Events;
using Core.Abstractions.Messaging.Transport;
using Core.Abstractions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Reflection;

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
    private readonly List<IChannel> _channels = new();

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
    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        var messageTypes = AppDomain.CurrentDomain.GetAssemblies().GetHandledIntegrationEventTypes();
        var factory = _serviceProvider.GetRequiredService<IQueueReferenceFactory>();

        foreach (var messageType in messageTypes)
        {
            MethodInfo methodInfo = typeof(IQueueReferenceFactory).GetMethod("Create");
            MethodInfo generic = methodInfo.MakeGenericMethod(messageType);

            var queueReferences = generic.Invoke(factory, new object[] { null }) as QueueReferences;
            var channel = await InitChannelAsync(queueReferences);
            _channels.Add(channel);
            await InitSubscriptionAsync(queueReferences, channel);
        }
    }


    /// <summary>
    /// Stops the consumer and closes all channels.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for stopping the consumer.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        foreach (var channel in _channels)
        {
            await StopChannelAsync(channel);
        }
    }


    /// <summary>
    /// Disposes the consumer and releases all resources.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        foreach (var channel in _channels)
        {
            await StopChannelAsync(channel);
        }
    }



    /// <summary>
    /// Initializes a subscription for a specific queue.
    /// </summary>
    /// <param name="queueReferences">Queue reference information.</param>
    /// <param name="channel">The RabbitMQ channel.</param>
    private async Task InitSubscriptionAsync(QueueReferences queueReferences, IChannel channel)
    {
        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += OnMessageReceivedAsync;

        _logger.LogInformation("Initializing subscription on queue '{QueueName}'...", queueReferences.QueueName);

        await channel.BasicConsumeAsync(queue: queueReferences.QueueName, autoAck: false, consumer: consumer);
    }


    /// <summary>
    /// Initializes a channel with the specified queue configuration.
    /// </summary>
    /// <param name="queueReferences">Queue reference information.</param>
    /// <returns>The initialized RabbitMQ channel.</returns>
    private async Task<IChannel> InitChannelAsync(QueueReferences queueReferences)
    {
        var channel = await _connection.CreateChannelAsync();

        _logger.LogInformation(
            "Initializing queue '{QueueName}' on exchange '{ExchangeName}'...",
            queueReferences.QueueName, queueReferences.ExchangeName);

        await channel.ExchangeDeclareAsync(exchange: queueReferences.ExchangeName, type: ExchangeType.Topic);
        _ = await channel.QueueDeclareAsync(queue: queueReferences.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: new Dictionary<string, object>
            {
            { Headers.XDeadLetterExchange, queueReferences.DeadLetterExchangeName },
            { Headers.XDeadLetterRoutingKey, queueReferences.DeadLetterQueue }
            });

        await channel.QueueBindAsync(queue: queueReferences.QueueName,
            exchange: queueReferences.ExchangeName,
            routingKey: queueReferences.RoutingKey,
            arguments: null);

        channel.CallbackExceptionAsync += OnChannelExceptionAsync;

        return channel;
    }


    /// <summary>
    /// Handles channel exceptions.
    /// </summary>
    /// <param name="args">CallbackExceptionEventArgs event arguments.</param>
    private Task OnChannelExceptionAsync(object sender, CallbackExceptionEventArgs args)
    {
        _logger.LogError(args.Exception,
            "RabbitMQ channel encountered an exception: {Message}",
            args.Exception.Message);

        return Task.CompletedTask;
    }



    /// <summary>
    /// Handles received messages asynchronously.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="eventArgs">Event arguments containing the message data.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task OnMessageReceivedAsync(object sender, BasicDeliverEventArgs eventArgs)
    {
        var consumer = sender as AsyncEventingBasicConsumer;
        var channel = consumer?.Channel;

        IIntegrationEvent message;
        try
        {
            message = _messageParser.Resolve(eventArgs.BasicProperties, eventArgs.Body.ToArray());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error decoding queue message from Exchange '{ExchangeName}': {ExceptionMessage}",
                eventArgs.Exchange,
                ex.Message);

            if (channel != null)
                await channel.BasicRejectAsync(eventArgs.DeliveryTag, requeue: false);

            return;
        }

        _logger.LogInformation(
            "Received message '{MessageId}' from Exchange '{ExchangeName}'. Processing...",
            eventArgs.BasicProperties.MessageId,
            eventArgs.Exchange);

        try
        {
            using var scope = _serviceProvider.CreateScope();
            var eventProcessor = scope.ServiceProvider.GetRequiredService<IEventProcessor>();

            await eventProcessor.DispatchAsync(message, default);
            await channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
        }
        catch (Exception ex)
        {
            await HandleConsumerExceptionAsync(ex, eventArgs, channel, message);
        }
    }



    /// <summary>
    /// Handles consumer exceptions during message processing.
    /// </summary>
    /// <param name="ex">The exception that occurred.</param>
    /// <param name="deliveryProps">Delivery properties of the message.</param>
    /// <param name="channel">The RabbitMQ channel.</param>
    /// <param name="message">The integration event being processed.</param>
    private async Task HandleConsumerExceptionAsync(
    Exception ex,
    BasicDeliverEventArgs deliveryProps,
    IChannel channel,
    IIntegrationEvent message)
    {
        _logger.LogWarning(
            "Error processing message '{MessageId}' from Exchange '{ExchangeName}': {ExceptionMessage}",
            message.EventId,
            deliveryProps.Exchange,
            ex.Message);

        await channel.BasicRejectAsync(deliveryProps.DeliveryTag, requeue: false);
    }


    /// <summary>
    /// Stops and disposes a channel.
    /// </summary>
    /// <param name="channel">The channel to stop.</param>
    private async Task StopChannelAsync(IChannel channel)
    {
        if (channel is null)
            return;

        channel.CallbackExceptionAsync -= OnChannelExceptionAsync;

        if (channel.IsOpen)
            await channel.CloseAsync();

        channel.Dispose();
    }


}
