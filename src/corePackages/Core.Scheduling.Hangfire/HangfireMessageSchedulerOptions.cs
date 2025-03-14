namespace Core.Scheduling.Hangfire;

public class HangfireMessageSchedulerOptions
{
    public string ConnectionString { get; set; }
    public bool UseInMemoryStorage { get; set; }
}
