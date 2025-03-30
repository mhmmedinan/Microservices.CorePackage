using Core.Abstractions.Messaging.Transport;
using Core.Messaging.Transport.RabbitMQ.Consumers;
using Core.Messaging.Transport.RabbitMQ.Producers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace Core.Messaging.Transport.RabbitMQ;

public static class RabbitMQExtensions
{
    public static IServiceCollection AddRabbitMqTransport(
       this IServiceCollection services,
       IConfiguration configuration,
       Action<RabbitConfiguration> configurator = null)
    {
        services.AddSingleton<IQueueReferenceFactory, QueueReferenceFactory>();
        services.AddSingleton<IMessageParser, MessageParser>();
        services.AddSingleton<IEventBusPublisher, RabbitMQProducer>();
        services.AddSingleton<IEventBusSubscriber, RabbitMQConsumer>();
        services.AddSingleton<IPublisherChannelContextPool, PublisherChannelContextPool>();
        services.AddSingleton<IPublisherChannelFactory, PublisherChannelFactory>();

        services.Configure<RabbitConfiguration>(configuration.GetSection(nameof(RabbitConfiguration)));
        if (configurator is { })
            services.Configure(nameof(RabbitConfiguration), configurator);

        var config = configuration.GetSection(nameof(RabbitConfiguration)).Get<RabbitConfiguration>();

        services.AddSingleton<IConnectionFactory>(ctx =>
        {
            var connectionFactory = new ConnectionFactory
            {
                HostName = config.HostName,
                UserName = config.UserName,
                Password = config.Password,
                Port = config.Port
            };
            return connectionFactory;
        });

        services.AddSingleton<IBusConnection, RabbitPersistentConnection>();
        services.AddSingleton(config);

        return services;
    }
}
