namespace Core.ElasticSearch.Models;

/// <summary>
/// Represents parameters for performing a search operation in Elasticsearch.
/// </summary>
public class SearchParameters
{
    /// <summary>
    /// Gets or sets the name of the index to search in.
    /// </summary>
    public string IndexName { get; set; }

    /// <summary>
    /// Gets or sets the starting position of the search results (for pagination).
    /// </summary>
    public int From { get; set; } = 0;

    /// <summary>
    /// Gets or sets the maximum number of results to return (for pagination).
    /// </summary>
    public int Size { get; set; } = 10;

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchParameters"/> class.
    /// </summary>
    public SearchParameters()
    {
        IndexName = string.Empty;
    }

    public SearchParameters(string indexName, int from, int size)
    {
        IndexName = indexName;
        From = from;
        Size = size;
    }
}