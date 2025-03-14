namespace Core.ElasticSearch.Models;

/// <summary>
/// Represents parameters for performing a field-based search in Elasticsearch.
/// </summary>
public class SearchByFieldParameters : SearchParameters
{
    /// <summary>
    /// Gets or sets the name of the field to search in.
    /// </summary>
    public string FieldName { get; set; }

    /// <summary>
    /// Gets or sets the value to search for in the specified field.
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchByFieldParameters"/> class.
    /// </summary>
    public SearchByFieldParameters()
    {
        FieldName = string.Empty;
        Value = string.Empty;
    }

    public SearchByFieldParameters(string fieldName, string value)
    {
        FieldName = fieldName;
        Value = value;
    }
}