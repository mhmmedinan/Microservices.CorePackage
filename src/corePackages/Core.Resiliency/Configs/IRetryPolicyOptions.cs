namespace Core.Resiliency.Configs;

public interface IRetryPolicyOptions
{
    int RetryCount { get; set; }
}

