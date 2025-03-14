namespace Core.Application.Pipelines.Performance;

/// <summary>
/// Interface for requests that require performance monitoring.
/// Defines the interval threshold for performance tracking.
/// </summary>
public interface IPerformanceRequest
{
    /// <summary>
    /// Gets the interval threshold in seconds.
    /// Operations taking longer than this interval will trigger performance alerts.
    /// </summary>
   int ThresholdInMilliseconds { get; }
}
