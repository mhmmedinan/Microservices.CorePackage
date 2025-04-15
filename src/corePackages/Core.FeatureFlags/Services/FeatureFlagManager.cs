using Core.FeatureFlags.Domain;
using Core.FeatureFlags.Repositories;

namespace Core.FeatureFlags.Services;

public class FeatureFlagManager : IFeatureFlagService
{
    private readonly IFeatureFlagRepository _repository;

    public FeatureFlagManager(IFeatureFlagRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> IsEnabledAsync(string key)
    {
        var flag = await _repository.GetAsync(key);
        return flag?.IsEnabled ?? false;
    }

    public async Task<FeatureFlag?> GetAsync(string key)
    {
        return await _repository.GetAsync(key);
    }

    public async Task<List<FeatureFlag>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task SetAsync(FeatureFlag flag)
    {
        await _repository.SetAsync(flag);
    }

    public async Task DeleteAsync(string key)
    {
        await _repository.DeleteAsync(key);

    }
}
