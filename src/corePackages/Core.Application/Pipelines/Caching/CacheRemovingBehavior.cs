using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text;

namespace Core.Application.Pipelines.Caching;

/// <summary>
/// Implements cache removal behavior for the MediatR pipeline.
/// Handles the removal of cached items and cache groups.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public class CacheRemovingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, ICacheRemoverRequest
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<CacheRemovingBehavior<TRequest, TResponse>> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CacheRemovingBehavior{TRequest, TResponse}"/> class.
    /// </summary>
    public CacheRemovingBehavior(IDistributedCache cache, ILogger<CacheRemovingBehavior<TRequest, TResponse>> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    /// <summary>
    /// Handles the request by implementing cache removal logic.
    /// </summary>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request.BypassCache)
        {
            _logger.LogInformation("Cache removal bypassed for {RequestType}", typeof(TRequest).Name);
            return await next();
        }

        TResponse response = await next();

        try
        {
            await RemoveCacheItems(request, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove cache items for {RequestType}", typeof(TRequest).Name);
        }

        return response;
    }

    private async Task RemoveCacheItems(TRequest request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(request.CacheGroupKey))
        {
            await RemoveCacheGroup(request.CacheGroupKey, cancellationToken);
        }

        if (!string.IsNullOrEmpty(request.CacheKey))
        {
            await RemoveSingleCacheItem(request.CacheKey, cancellationToken);
        }
    }

    private async Task RemoveCacheGroup(string groupKey, CancellationToken cancellationToken)
    {
        try
        {
            byte[]? cachedGroup = await _cache.GetAsync(groupKey, cancellationToken);
            if (cachedGroup == null)
            {
                _logger.LogInformation("Cache group not found: {GroupKey}", groupKey);
                return;
            }

            var keysInGroup = await DeserializeCacheGroup(cachedGroup, groupKey);
            if (keysInGroup == null) return;

            foreach (string key in keysInGroup)
            {
                await RemoveSingleCacheItem(key, cancellationToken);
            }

            await RemoveSingleCacheItem(groupKey, cancellationToken);
            _logger.LogInformation("Removed cache group: {GroupKey}", groupKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove cache group: {GroupKey}", groupKey);
            throw;
        }
    }

    private async Task<HashSet<string>?> DeserializeCacheGroup(byte[] cachedGroup, string groupKey)
    {
        try
        {
            return JsonSerializer.Deserialize<HashSet<string>>(Encoding.UTF8.GetString(cachedGroup));
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize cache group: {GroupKey}", groupKey);
            return null;
        }
    }

    private async Task RemoveSingleCacheItem(string key, CancellationToken cancellationToken)
    {
        try
        {
            await _cache.RemoveAsync(key, cancellationToken);
            _logger.LogInformation("Removed cache item: {Key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove cache item: {Key}", key);
            throw;
        }
    }
}