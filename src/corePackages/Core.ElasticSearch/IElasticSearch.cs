using Core.ElasticSearch.Models;
using Nest;

namespace Core.ElasticSearch;

/// <summary>
/// Provides operations for interacting with Elasticsearch.
/// </summary>
public interface IElasticSearch
{
    /// <summary>
    /// Creates a new index in Elasticsearch.
    /// </summary>
    /// <param name="indexModel">The model containing index creation parameters.</param>
    /// <returns>Result of the index creation operation.</returns>
    Task<IElasticSearchResult> CreateNewIndexAsync(IndexModel indexModel);

    /// <summary>
    /// Inserts a single document into Elasticsearch.
    /// </summary>
    /// <param name="model">The model containing the document to insert.</param>
    /// <returns>Result of the insert operation.</returns>
    Task<IElasticSearchResult> InsertAsync(ElasticSearchInsertUpdateModel model);

    /// <summary>
    /// Inserts multiple documents into Elasticsearch.
    /// </summary>
    /// <param name="indexName">Name of the index to insert documents into.</param>
    /// <param name="items">Array of documents to insert.</param>
    /// <returns>Result of the bulk insert operation.</returns>
    Task<IElasticSearchResult> InsertManyAsync(string indexName, object[] items);

    /// <summary>
    /// Gets a list of all indices in the Elasticsearch cluster.
    /// </summary>
    /// <returns>Dictionary of index names and their states.</returns>
    IReadOnlyDictionary<IndexName, IndexState> GetIndexList();

    /// <summary>
    /// Performs a search across all documents in an index.
    /// </summary>
    /// <typeparam name="T">Type of the documents to search for.</typeparam>
    /// <param name="parameters">Search parameters including index name and other options.</param>
    /// <returns>List of matching documents.</returns>
    Task<List<ElasticSearchGetModel<T>>> GetAllSearch<T>(SearchParameters parameters)
        where T : class;

    /// <summary>
    /// Performs a field-based search in Elasticsearch.
    /// </summary>
    /// <typeparam name="T">Type of the documents to search for.</typeparam>
    /// <param name="fieldParameters">Parameters specifying the field and value to search for.</param>
    /// <returns>List of matching documents.</returns>
    Task<List<ElasticSearchGetModel<T>>> GetSearchByField<T>(SearchByFieldParameters fieldParameters)
        where T : class;

    /// <summary>
    /// Performs a search using Elasticsearch's simple query string syntax.
    /// </summary>
    /// <typeparam name="T">Type of the documents to search for.</typeparam>
    /// <param name="queryParameters">Parameters containing the query string and other options.</param>
    /// <returns>List of matching documents.</returns>
    Task<List<ElasticSearchGetModel<T>>> GetSearchBySimpleQueryString<T>(SearchByQueryParameters queryParameters)
        where T : class;

    /// <summary>
    /// Updates a document in Elasticsearch by its ID.
    /// </summary>
    /// <param name="model">The model containing the updated document and its ID.</param>
    /// <returns>Result of the update operation.</returns>
    Task<IElasticSearchResult> UpdateByElasticIdAsync(ElasticSearchInsertUpdateModel model);

    /// <summary>
    /// Deletes a document from Elasticsearch by its ID.
    /// </summary>
    /// <param name="model">The model containing the document ID to delete.</param>
    /// <returns>Result of the delete operation.</returns>
    Task<IElasticSearchResult> DeleteByElasticIdAsync(ElasticSearchModel model);
}
