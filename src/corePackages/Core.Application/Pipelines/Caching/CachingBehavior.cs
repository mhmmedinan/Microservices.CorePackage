using MediatR;
using System.Text.Json;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using System.Net.Sockets;
using System.IO.Compression;
using Core.CrossCuttingConcerns.Exceptions.Types;

namespace Core.Application.Pipelines.Caching;

public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, ICachableRequest
{
    private readonly IDistributedCache _cache;
    private readonly CacheSettings _cacheSettings;
    private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger;
    private int _retryCount;

    public CachingBehavior(
        IDistributedCache cache, 
        ILogger<CachingBehavior<TRequest, TResponse>> logger, 
        IConfiguration configuration)
    {
        _cache = cache;
        _logger = logger;
        _cacheSettings = configuration.GetSection("CacheSettings").Get<CacheSettings>() 
            ?? throw new BusinessException("Cache settings not found in configuration.");
        _retryCount = 0;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request.BypassCache || !_cacheSettings.Enabled)
            return await next();

        try
        {
            byte[]? cachedResponse = await _cache.GetAsync(request.CacheKey, cancellationToken);
            if (cachedResponse != null)
            {
                if (request.CompressData)
                    cachedResponse = await DecompressDataAsync(cachedResponse);

                var response = JsonSerializer.Deserialize<TResponse>(Encoding.Default.GetString(cachedResponse));
                if (response != null)
                {
                    _logger.LogInformation("Cache hit for key: {CacheKey}", request.CacheKey);
                    return response;
                }
            }

            return await GetResponseAndAddToCache(request, next, cancellationToken);
        }
        catch (Exception ex) when (IsCacheConnectionError(ex) && _retryCount < _cacheSettings.MaxRetryAttempts)
        {
            _retryCount++;
            _logger.LogWarning(ex, "Cache operation failed. Retrying {RetryCount} of {MaxRetryAttempts}", 
                _retryCount, _cacheSettings.MaxRetryAttempts);

            await Task.Delay(TimeSpan.FromMilliseconds(_cacheSettings.RetryDelayMilliseconds * _retryCount), cancellationToken);
            return await Handle(request, next, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Cache error occurred. Bypassing cache.");
            return await next();
        }
    }

    private async Task<TResponse> GetResponseAndAddToCache(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        TResponse response = await next();
        if (response == null)
            return response!;

        try
        {
            var cacheOptions = new DistributedCacheEntryOptions();
            switch (request.Strategy)
            {
                case CacheStrategy.Sliding:
                    cacheOptions.SlidingExpiration = request.SlidingExpiration ?? TimeSpan.FromDays(_cacheSettings.SlidingExpiration);
                    break;
                case CacheStrategy.Absolute:
                    cacheOptions.AbsoluteExpiration = DateTimeOffset.UtcNow.Add(
                        request.AbsoluteExpiration ?? TimeSpan.FromDays(_cacheSettings.SlidingExpiration));
                    break;
            }

            byte[] serializedData = JsonSerializer.SerializeToUtf8Bytes(response);
            if (request.CompressData)
                serializedData = await CompressDataAsync(serializedData);

            await _cache.SetAsync(request.CacheKey, serializedData, cacheOptions, cancellationToken);
            _logger.LogInformation("Added to cache: {CacheKey}", request.CacheKey);

            if (!string.IsNullOrEmpty(request.CacheGroupKey))
                await AddCacheKeyToGroup(request, cacheOptions.SlidingExpiration ?? TimeSpan.FromDays(_cacheSettings.SlidingExpiration), cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add response to cache");
        }

        return response;
    }

    private async Task AddCacheKeyToGroup(TRequest request, TimeSpan slidingExpiration, CancellationToken cancellationToken)
    {
        try
        {
            byte[]? cacheGroupCache = await _cache.GetAsync(request.CacheGroupKey!, cancellationToken);
            HashSet<string> cacheKeysInGroup = cacheGroupCache != null
                ? JsonSerializer.Deserialize<HashSet<string>>(Encoding.Default.GetString(cacheGroupCache))!
                : new HashSet<string>();

            if (!cacheKeysInGroup.Contains(request.CacheKey))
            {
                cacheKeysInGroup.Add(request.CacheKey);
                await _cache.SetAsync(
                    request.CacheGroupKey!,
                    JsonSerializer.SerializeToUtf8Bytes(cacheKeysInGroup),
                    new DistributedCacheEntryOptions { SlidingExpiration = slidingExpiration },
                    cancellationToken
                );
                _logger.LogInformation("Added to cache group: {GroupKey}", request.CacheGroupKey);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add key to cache group");
        }
    }

    private bool IsCacheConnectionError(Exception ex) =>
        ex is RedisConnectionException or SocketException or ObjectDisposedException ||
        (ex is InvalidOperationException && ex.Message.Contains("ConnectionMultiplexer"));

    private async Task<byte[]> CompressDataAsync(byte[] data)
    {
        using var outputStream = new MemoryStream();
        using (var gzipStream = new GZipStream(outputStream, CompressionLevel.Optimal))
            await gzipStream.WriteAsync(data);
        return outputStream.ToArray();
    }

    private async Task<byte[]> DecompressDataAsync(byte[] compressedData)
    {
        using var inputStream = new MemoryStream(compressedData);
        using var outputStream = new MemoryStream();
        using (var gzipStream = new GZipStream(inputStream, CompressionMode.Decompress))
            await gzipStream.CopyToAsync(outputStream);
        return outputStream.ToArray();
    }
}
