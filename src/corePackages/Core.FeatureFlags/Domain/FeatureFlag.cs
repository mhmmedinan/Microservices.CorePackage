using Core.Persistence.Repositories.MongoDb;

namespace Core.FeatureFlags.Domain;

public class FeatureFlag:MongoDbEntity
{
    public string Key { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsEnabled { get; set; }
    public Dictionary<string, object>? Conditions { get; set; }
}