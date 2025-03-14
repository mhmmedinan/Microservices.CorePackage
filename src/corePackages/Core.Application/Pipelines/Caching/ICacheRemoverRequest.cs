namespace Core.Application.Pipelines.Caching;

/// <summary>
/// Interface for requests that remove items from the cache.
/// Provides configuration options for cache removal operations.
/// </summary>
public interface ICacheRemoverRequest
{
    /// <summary>
    /// Gets whether to bypass the cache removal operation.
    /// When true, no cache items will be removed.
    /// </summary>
    bool BypassCache { get; }

    /// <summary>
    /// Gets the key of the cache item to remove.
    /// If null, no individual cache item will be removed.
    /// </summary>
    string? CacheKey { get; }

    /// <summary>
    /// Gets the key of the cache group to remove.
    /// If null, no cache group will be removed.
    /// </summary>
    string? CacheGroupKey { get; }
}
