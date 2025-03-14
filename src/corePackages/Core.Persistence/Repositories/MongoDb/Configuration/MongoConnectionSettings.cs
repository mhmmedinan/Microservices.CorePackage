using MongoDB.Driver;

namespace Core.Persistence.Repositories.MongoDb.Configuration;

public class MongoConnectionSettings
{
    public MongoConnectionSettings()
    {
        
    }

    public MongoConnectionSettings(MongoClientSettings mongoClientSettings)
    {
        MongoClientSettings = mongoClientSettings;
    }


    public string ConnectionString { get; set; }
    public string DatabaseName { get; set; }

    private MongoClientSettings MongoClientSettings { get; set; }

    public MongoClientSettings GetMongoClientSettings()
    {
        return MongoClientSettings;
    }

}
