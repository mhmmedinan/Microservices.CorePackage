namespace Core.Abstractions.Types;

/// <summary>
/// Defines system-wide information and configuration settings.
/// Provides access to essential system properties that identify and configure the application instance.
/// </summary>
/// <remarks>
/// Use this interface to access:
/// <list type="bullet">
/// <item>Client identification information</item>
/// <item>System configuration settings</item>
/// <item>Runtime environment properties</item>
/// <item>Application instance details</item>
/// </list>
/// Common scenarios:
/// <list type="bullet">
/// <item>Distributed system identification</item>
/// <item>Client group management</item>
/// <item>System behavior configuration</item>
/// <item>Instance tracking</item>
/// </list>
/// </remarks>
public interface ISystemInfo
{
    /// <summary>
    /// Gets the group identifier for the client.
    /// Used for grouping related clients or instances in a distributed system.
    /// </summary>
    string ClientGroup { get; }

    /// <summary>
    /// Gets the unique identifier for this client instance.
    /// Ensures each instance can be uniquely identified in the system.
    /// </summary>
    Guid ClientId { get; }

    /// <summary>
    /// Gets a value indicating whether this instance operates in publish-only mode.
    /// When true, the instance only publishes events/messages but does not process them.
    /// </summary>
    bool PublishOnly { get; }
}
