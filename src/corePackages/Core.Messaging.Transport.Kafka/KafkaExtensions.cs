using Core.Abstractions.Messaging.Transport;
using Core.Messaging.Transport.Kafka.Consumer;
using Core.Messaging.Transport.Kafka.Producers;
using Core.Messaging.Transport.Kafka.SchemaRegistry;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Messaging.Transport.Kafka;

public static class KafkaExtensions
{
    /// <summary>
    /// Registers Kafka producer, consumer and schema registry services into the DI container.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">Application configuration</param>
    /// <returns>The updated service collection</returns>
    public static IServiceCollection AddKafkaMessaging(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddSingleton(configuration.GetKafkaProducerConfig());
        services.AddSingleton(configuration.GetKafkaConsumerConfig());

        // Producer ve Consumer
        services.AddSingleton<IEventBusPublisher, KafkaProducer>();
        services.AddSingleton<IEventBusSubscriber, KafkaConsumer>();

        // Schema Registry client
        services.AddSchemeRegistry(configuration);

        return services;
    }
}
