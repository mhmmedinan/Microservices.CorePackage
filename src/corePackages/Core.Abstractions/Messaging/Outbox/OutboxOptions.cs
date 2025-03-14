namespace Core.Abstractions.Messaging.Outbox;

public class OutboxOptions
{
    public string ConnectionString { get; set; }
    public bool Enabled { get; set; } = true;
    public TimeSpan? Interval { get; set; }
    public bool UseBackgroundDispatcher { get; set; } = true;
}
