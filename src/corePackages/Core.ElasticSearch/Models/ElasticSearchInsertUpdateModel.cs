using Nest;

namespace Core.ElasticSearch.Models;

/// <summary>
/// Represents a model for inserting or updating documents in Elasticsearch.
/// </summary>
public class ElasticSearchInsertUpdateModel : ElasticSearchModel
{
    /// <summary>
    /// Gets or sets the document to insert or update.
    /// </summary>
    public object Item { get; set; }

     /// <summary>
    /// Initializes a new instance of the <see cref="ElasticSearchInsertUpdateModel"/> class.
    /// </summary>

    public ElasticSearchInsertUpdateModel(object item)
    {
        Item = item;
    }

    public ElasticSearchInsertUpdateModel(string elasticId, string indexName, object item)
        : base(elasticId, indexName)
    {
        Item = item;
    }
}