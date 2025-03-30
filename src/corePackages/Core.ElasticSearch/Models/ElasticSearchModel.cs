using Nest;

namespace Core.ElasticSearch.Models;

/// <summary>
/// Base model for Elasticsearch operations that require an index name and document ID.
/// </summary>
public class ElasticSearchModel
{
    /// <summary>
    /// Gets or sets the name of the index to operate on.
    /// </summary>
    public string IndexName { get; set; }

    /// <summary>
    /// Gets or sets the ID of the document in Elasticsearch.
    /// </summary>
    public string ElasticId { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ElasticSearchModel"/> class.
    /// </summary>
    public ElasticSearchModel()
    {
        IndexName = string.Empty;
        ElasticId = string.Empty;
    }

    public ElasticSearchModel(string elasticId, string indexName)
    {
        ElasticId = elasticId;
        IndexName = indexName;
    }
}