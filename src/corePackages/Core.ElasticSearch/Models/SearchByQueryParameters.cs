namespace Core.ElasticSearch.Models;

/// <summary>
/// Represents parameters for performing a query string search in Elasticsearch.
/// </summary>
public class SearchByQueryParameters : SearchParameters
{
    public string QueryName { get; set; }
    public string Query { get; set; }
    public string[] Fields { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchByQueryParameters"/> class.
    /// </summary>
    public SearchByQueryParameters()
    {
        QueryName = string.Empty;
        Query = string.Empty;
        Fields = Array.Empty<string>();
    }

    public SearchByQueryParameters(string queryName, string query, string[] fields)
    {
        QueryName = queryName;
        Query = query;
        Fields = fields;
    }
}