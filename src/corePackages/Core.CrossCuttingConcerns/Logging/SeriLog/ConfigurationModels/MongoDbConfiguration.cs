namespace Core.CrossCuttingConcerns.Logging.SeriLog.ConfigurationModels;

public class MongoDbConfiguration
{
    public string ConnectionString { get; set; }
    public string Collection { get; set; }

    public MongoDbConfiguration()
    {
        ConnectionString = string.Empty;
        Collection = string.Empty;
    }

    public MongoDbConfiguration(string connectionString, string collection)
    {
        ConnectionString = connectionString;
        Collection = collection;
    }
}
