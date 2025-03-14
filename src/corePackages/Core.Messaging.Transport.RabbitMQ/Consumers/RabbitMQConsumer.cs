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

public class RabbitMQConsumer : IEventBusSubscriber
{
    private readonly IBusConnection _connection;
    private readonly IMessageParser _messageParser;
    private readonly ILogger<RabbitMQConsumer> _logger; 
    private readonly RabbitConfiguration _rabbitConfiguration;
    private readonly IServiceProvider _serviceProvider;
    private readonly List<IModel> _channels = new();


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

    public Task StopAsync(CancellationToken cancellationToken = default)
    {
        _channels.ForEach(StopChannel);
        return Task.CompletedTask;
    }

    private void InitSubscription(QueueReferences queueReferences, IModel channel)
    {
        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.Received += OnMessageReceivedAsync;

        _logger.LogInformation($"initializing subscription on queue '{queueReferences.QueueName}' ...");
        channel.BasicConsume(queue: queueReferences.QueueName, autoAck: false, consumer: consumer);
    }

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

    private void OnChannelException(object _, CallbackExceptionEventArgs ea)
    {
        _logger.LogError(ea.Exception, "the RabbitMQ Channel has encountered an error: {ExceptionMessage}",
            ea.Exception.Message);

        // TODO --> Provide argument
        // InitChannel();
        // InitSubscription();
    }

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

    public void Dispose()
    {
        _channels.ForEach(StopChannel);
    }
}
