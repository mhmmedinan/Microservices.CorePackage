namespace Core.Scheduling.SqlServer.Internal;

public class InternalMessageSchedulerOptions
{
    public bool Enabled { get; set; }
    public string ConnectionString { get; set; }
    public TimeSpan? Interval { get; set; }
}

