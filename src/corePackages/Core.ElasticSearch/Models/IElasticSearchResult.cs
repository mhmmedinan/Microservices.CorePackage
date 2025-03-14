namespace Core.ElasticSearch.Models;

/// <summary>
/// Defines the contract for Elasticsearch operation results.
/// </summary>
public interface IElasticSearchResult
{
    /// <summary>
    /// Gets a value indicating whether the operation was successful.
    /// </summary>
    bool Success { get; }

    /// <summary>
    /// Gets the message describing the result of the operation.
    /// </summary>
    string Message { get; }
}