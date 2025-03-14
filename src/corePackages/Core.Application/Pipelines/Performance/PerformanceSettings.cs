namespace Core.Application.Pipelines.Performance;

/// <summary>
/// Configuration settings for performance monitoring behavior.
/// </summary>
public class PerformanceSettings
{
    /// <summary>
    /// Gets or sets the threshold in milliseconds for performance monitoring.
    /// Operations taking longer than this threshold will trigger alerts.
    /// </summary>
    public int ThresholdInMilliseconds { get; set; }

    /// <summary>
    /// Gets or sets whether to enable email alerts for performance issues.
    /// </summary>
    public bool EnableEmailAlerts { get; set; }

    /// <summary>
    /// Gets or sets the email address to send performance alerts to.
    /// </summary>
    public string AlertEmailAddress { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the display name for the alert email recipient.
    /// </summary>
    public string AlertEmailDisplayName { get; set; } = string.Empty;
} 