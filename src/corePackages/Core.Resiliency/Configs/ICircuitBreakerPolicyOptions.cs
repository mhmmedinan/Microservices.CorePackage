namespace Core.Resiliency.Configs;

public interface ICircuitBreakerPolicyOptions
{
    int RetryCount { get; set; }
    int BreakDuration { get; set; }
}
