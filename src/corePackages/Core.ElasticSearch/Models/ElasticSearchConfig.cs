namespace Core.ElasticSearch.Models;

/// <summary>
/// Configuration settings for Elasticsearch connection and operations.
/// </summary>
public class ElasticSearchConfig
{
    /// <summary>
    /// Gets or sets the connection string for Elasticsearch.
    /// </summary>
    public string ConnectionString { get; set; }

    /// <summary>
    /// Gets or sets the username for authentication.
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// Gets or sets the password for authentication.
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ElasticSearchConfig"/> class.
    /// </summary>
    public ElasticSearchConfig()
    {
        ConnectionString = string.Empty;
        UserName = string.Empty;
        Password = string.Empty;
    }

    public ElasticSearchConfig(string connectionString, string userName, string password)
    {
        ConnectionString = connectionString;
        UserName = userName;
        Password = password;
    }
}