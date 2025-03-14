using Ardalis.GuardClauses;
using Confluent.Kafka;
using Core.Abstractions.Events;
using Core.Abstractions.Events.External;
using Core.Abstractions.Messaging.Transport;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Core.Messaging.Transport.Kafka.Producers;

public class KafkaProducer : IEventBusPublisher
{
    private readonly KafkaProducerConfig _config;

    public KafkaProducer(IConfiguration configuration)
    {
        Guard.Against.Null(configuration, nameof(configuration));

        _config = configuration.GetKafkaProducerConfig();
    }


    public async Task PublishAsync<TEvent>(TEvent integrationEvent, CancellationToken cancellationToken = default)
        where TEvent : IIntegrationEvent
    {
        using (var p = new ProducerBuilder<string, string>(_config.ProducerConfig).Build())
        {
            await Task.Yield();

            var data = JsonConvert.SerializeObject(integrationEvent);

            // publish event to kafka topic taken from config
            await p.ProduceAsync(
                _config.Topic,
                new Message<string, string>
                {
                    // store event type name in message Key
                    Key = integrationEvent.GetType().Name,

                    // serialize event to message Value
                    Value = data
                },
                cancellationToken);
        }
    }
}
