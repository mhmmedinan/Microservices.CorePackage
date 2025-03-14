namespace Core.Scheduling.SqlServer.Internal;

public class InternalMessage
{
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets name of message.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Gets the date the message occurred.
    /// </summary>
    public DateTime OccurredOn { get; private set; }

    /// <summary>
    /// Gets the command type full name.
    /// </summary>
    public string Type { get; private set; }

    /// <summary>
    /// Gets the command data - serialized to JSON.
    /// </summary>
    public string Data { get; private set; }

    /// <summary>
    /// Gets the date the message processed.
    /// </summary>
    public DateTime? ProcessedOn { get; private set; }


    /// <summary>
    /// Gets the CorrelationId of our command.
    /// </summary>
    public string? CorrelationId { get; private set; }


    public InternalMessage(
        Guid id,
        DateTime occurredOn,
        string type,
        string name,
        string data,
        string correlationId = null)
    {
        OccurredOn = occurredOn;
        Type = type;
        Data = data;
        Id = id;
        Name = name;
        CorrelationId = correlationId;
    }

    public void MarkAsProcessed()
    {
        ProcessedOn = DateTime.Now;
    }
}
