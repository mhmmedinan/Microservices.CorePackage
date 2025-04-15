using Core.FeatureFlags.Domain;

namespace Core.FeatureFlags.Repositories;

public interface IFeatureFlagRepository
{
    Task<FeatureFlag?> GetAsync(string key);
    Task<List<FeatureFlag>> GetAllAsync();
    Task SetAsync(FeatureFlag flag);
    Task DeleteAsync(string key);
}
