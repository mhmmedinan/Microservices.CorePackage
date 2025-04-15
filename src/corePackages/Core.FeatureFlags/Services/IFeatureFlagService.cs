using Core.FeatureFlags.Domain;

namespace Core.FeatureFlags.Services;

public interface IFeatureFlagService
{
    Task<bool> IsEnabledAsync(string key);
    Task<FeatureFlag?> GetAsync(string key);
    Task<List<FeatureFlag>> GetAllAsync();
    Task SetAsync(FeatureFlag flag);
    Task DeleteAsync(string key);
}
