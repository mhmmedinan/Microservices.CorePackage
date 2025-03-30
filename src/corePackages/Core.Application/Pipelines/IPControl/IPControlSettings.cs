namespace Core.Application.Pipelines.IPControl;

/// <summary>
/// Represents IP control configuration settings.
/// </summary>
public class IPControlSettings
{
    /// <summary>
    /// List of allowed IP addresses (whitelist).
    /// Requests from these IPs are always permitted.
    /// </summary>
    public List<string>? WhiteList { get; set; }

    /// <summary>
    /// List of denied IP addresses (blacklist).
    /// Requests from these IPs are always rejected.
    /// </summary>
    public List<string>? BlackList { get; set; }
}
