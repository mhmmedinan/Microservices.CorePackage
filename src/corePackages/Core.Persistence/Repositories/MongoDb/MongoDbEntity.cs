using MongoDB.Bson;

namespace Core.Persistence.Repositories.MongoDb;

public class MongoDbEntity
{
    public ObjectId Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public DateTime? DeletedDate { get; set; }

    public MongoDbEntity()
    {
        Id = default!;
    }

    public MongoDbEntity(ObjectId id)
    {
        Id = id;
    }
}
