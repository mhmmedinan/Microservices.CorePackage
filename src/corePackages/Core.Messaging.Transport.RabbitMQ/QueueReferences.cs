namespace Core.Messaging.Transport.RabbitMQ;

/// <summary>
/// Represents the references and naming conventions for RabbitMQ queues and exchanges.
/// </summary>
/// <param name="ExchangeName">The name of the exchange.</param>
/// <param name="QueueName">The name of the queue.</param>
/// <param name="RoutingKey">The routing key for message routing.</param>
/// <param name="DeadLetterExchangeName">The name of the dead letter exchange.</param>
/// <param name="DeadLetterQueue">The name of the dead letter queue.</param>
public record QueueReferences(string ExchangeName, string QueueName, string RoutingKey,
    string DeadLetterExchangeName, string DeadLetterQueue)
{
    /// <summary>
    /// Gets the name of the retry exchange.
    /// </summary>
    public string RetryExchangeName => this.ExchangeName + ".retry";

    /// <summary>
    /// Gets the name of the retry queue.
    /// </summary>
    public string RetryQueueName => this.QueueName + ".retry";
}
