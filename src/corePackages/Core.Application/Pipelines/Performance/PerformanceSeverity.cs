namespace Core.Application.Pipelines.Performance;

/// <summary>
/// Defines severity levels for performance alerts.
/// </summary>
public enum PerformanceSeverity
{
    /// <summary>
    /// Warning level - Operation is slower than threshold
    /// </summary>
    Warning = 1,

    /// <summary>
    /// Error level - Operation is significantly slower than threshold
    /// </summary>
    Error = 2,

    /// <summary>
    /// Critical level - Operation is critically slow
    /// </summary>
    Critical = 3
} 