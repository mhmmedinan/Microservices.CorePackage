namespace Core.Messaging.Transport.RabbitMQ;

public record QueueReferences(string ExchangeName, string QueueName, string RoutingKey,
    string DeadLetterExchangeName, string DeadLetterQueue)
{
    public string RetryExchangeName => this.ExchangeName + ".retry";
    public string RetryQueueName => this.QueueName + ".retry";
}
