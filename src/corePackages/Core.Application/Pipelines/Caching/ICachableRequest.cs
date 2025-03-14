namespace Core.Application.Pipelines.Caching;

/// <summary>
/// Interface for requests that support caching in the MediatR pipeline.
/// Provides configuration options for cache behavior.
/// </summary>
public interface ICachableRequest
{
    /// <summary>
    /// Gets whether to bypass the cache for this request.
    /// When true, the request will not use cached data.
    /// </summary>
    bool BypassCache { get; }

    /// <summary>
    /// Gets the cache key for this request.
    /// Used to store and retrieve cached data.
    /// </summary>
    string? CacheKey { get; }

    /// <summary>
    /// Gets the cache group key for this request.
    /// Used for managing related cache entries.
    /// </summary>
    string? CacheGroupKey { get; }

    /// <summary>
    /// Gets the sliding expiration time for the cache entry.
    /// The cache entry will expire if not accessed within this timespan.
    /// </summary>
    TimeSpan? SlidingExpiration { get; }

    /// <summary>
    /// Gets the cache strategy for this request.
    /// </summary>
    CacheStrategy Strategy { get; }

    /// <summary>
    /// Gets the absolute expiration time for the cache entry.
    /// The cache entry will expire at this specific time.
    /// </summary>
    TimeSpan? AbsoluteExpiration { get; }

    /// <summary>
    /// Gets whether to compress the cached data.
    /// </summary>
    bool CompressData { get; }
}
