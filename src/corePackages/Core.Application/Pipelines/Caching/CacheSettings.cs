namespace Core.Application.Pipelines.Caching;

/// <summary>
/// Configuration settings for the caching behavior.
/// Defines cache-related parameters and options.
/// </summary>
public class CacheSettings
{
   /// <summary>
   /// Gets or sets a value indicating whether caching is enabled.
   /// </summary>
   public bool Enabled { get; set; } = true;

   /// <summary>
   /// Gets or sets the sliding expiration time in minutes.
   /// Cache entries will expire if not accessed within this timespan.
   /// </summary>
   public int SlidingExpiration { get; set; }

   /// <summary>
   /// Gets or sets the maximum number of retry attempts.
   /// </summary>
   public int MaxRetryAttempts { get; set; } = 3;

   /// <summary>
   /// Gets or sets the delay in milliseconds between retry attempts.
   /// </summary>
   public int RetryDelayMilliseconds { get; set; } = 300;
}
