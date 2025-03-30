using Core.CrossCuttingConcerns.Logging.SeriLog.ConfigurationModels;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Formatting.Json;
using Serilog.Sinks.RabbitMQ;

namespace Core.CrossCuttingConcerns.Logging.SeriLog.Logger;

public class RabbitMQLogger : LoggerServiceBase
{
    public RabbitMQLogger(IConfiguration configuration)
    {
        const string configurationSection = "SeriLogConfigurations:RabbitMQConfiguration";
        RabbitMQConfiguration rabbitMQConfiguration =
            configuration.GetSection(configurationSection).Get<RabbitMQConfiguration>()
            ?? throw new NullReferenceException($"\"{configurationSection}\" section cannot found in configuration.");

        Logger = new LoggerConfiguration()
            .WriteTo.RabbitMQ((clientConfiguration, sinkConfiguration) =>
            {
                clientConfiguration.Username = rabbitMQConfiguration.Username;
                clientConfiguration.Password = rabbitMQConfiguration.Password;
                clientConfiguration.Exchange = rabbitMQConfiguration.Exchange;
                clientConfiguration.ExchangeType = rabbitMQConfiguration.ExchangeType;
                clientConfiguration.DeliveryMode = RabbitMQDeliveryMode.Durable;
                clientConfiguration.RoutingKey = rabbitMQConfiguration.RouteKey;
                clientConfiguration.Port = rabbitMQConfiguration.Port;

              
                rabbitMQConfiguration.Hostnames.ForEach(hostname => clientConfiguration.Hostnames.Add(hostname));

                sinkConfiguration.TextFormatter = new JsonFormatter();
            })
            .CreateLogger();
    }
}
