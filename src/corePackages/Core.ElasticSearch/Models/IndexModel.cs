namespace Core.ElasticSearch.Models;

/// <summary>
/// Represents the configuration for creating a new Elasticsearch index.
/// </summary>
public class IndexModel
{
    /// <summary>
    /// Gets or sets the name of the index to be created.
    /// </summary>
    public string IndexName { get; set; }

    /// <summary>
    /// Gets or sets the alias name for the index.
    /// </summary>
    public string AliasName { get; set; }

    /// <summary>
    /// Gets or sets the number of shards for the index.
    /// </summary>
    public int NumberOfShards { get; set; } = 3;

    /// <summary>
    /// Gets or sets the number of replicas for the index.
    /// </summary>
    public int NumberOfReplicas { get; set; } = 1;

    /// <summary>
    /// Initializes a new instance of the <see cref="IndexModel"/> class.
    /// </summary>
    public IndexModel()
    {
        IndexName = string.Empty;
        AliasName = string.Empty;
    }

    public IndexModel(string indexName, string aliasName)
    {
        IndexName = indexName;
        AliasName = aliasName;
    }
}