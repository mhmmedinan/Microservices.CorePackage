namespace Core.ElasticSearch.Models;

/// <summary>
/// Represents a model for retrieving documents from Elasticsearch.
/// </summary>
/// <typeparam name="T">The type of the document being retrieved.</typeparam>
public class ElasticSearchGetModel<T>
{
    /// <summary>
    /// Gets or sets the Elasticsearch document ID.
    /// </summary>
    public string ElasticId { get; set; }

    /// <summary>
    /// Gets or sets the actual document data.
    /// </summary>
    public T Item { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ElasticSearchGetModel{T}"/> class.
    /// </summary>
    public ElasticSearchGetModel()
    {
        ElasticId = string.Empty;
        Item = default!;
    }

    public ElasticSearchGetModel(string elasticId, T item)
    {
        ElasticId = elasticId;
        Item = item;
    }
}