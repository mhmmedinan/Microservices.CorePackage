namespace Core.Resiliency.Configs;

public class PolicyOptions : ICircuitBreakerPolicyOptions, IRetryPolicyOptions, ITimeoutPolicyOptions
{
    public int RetryCount { get; set; }
    public int BreakDuration { get; set; }
    public int TimeOutDuration { get; set; }
}
