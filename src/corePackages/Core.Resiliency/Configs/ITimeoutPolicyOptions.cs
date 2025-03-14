namespace Core.Resiliency.Configs;

public interface ITimeoutPolicyOptions
{
    public int TimeOutDuration { get; set; }
}
