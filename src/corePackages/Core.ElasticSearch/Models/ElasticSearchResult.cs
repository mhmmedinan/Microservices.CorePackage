namespace Core.ElasticSearch.Models;

/// <summary>
/// Represents the result of an Elasticsearch operation.
/// </summary>
public class ElasticSearchResult : IElasticSearchResult
{
    /// <summary>
    /// Gets or sets a value indicating whether the operation was successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Gets or sets the message describing the result of the operation.
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ElasticSearchResult"/> class.
    /// </summary>
    public ElasticSearchResult()
    {
        Message = string.Empty;
    }

    public ElasticSearchResult(bool success, string? message = null)
    {
        Success = success;
        Message = message;
    }
}