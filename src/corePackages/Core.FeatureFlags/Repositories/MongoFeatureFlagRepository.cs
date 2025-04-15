using Core.FeatureFlags.Domain;
using Core.FeatureFlags.Repositories;
using Core.Persistence.Repositories.MongoDb;

namespace Core.FeatureFlags;

public class MongoFeatureFlagRepository : IFeatureFlagRepository
{
    private readonly IMongoAsyncRepository<FeatureFlag> _repository;

    public MongoFeatureFlagRepository(IMongoAsyncRepository<FeatureFlag> repository)
    {
        _repository = repository;
    }

    public async Task<FeatureFlag?> GetAsync(string key)
    {
        var list = await _repository.GetListAsync(x => x.Key == key);
        return list.FirstOrDefault();
    }

    public async Task<List<FeatureFlag>> GetAllAsync()
    {
        var list = await _repository.GetListAsync();
        return list.ToList();
    }

    public async Task SetAsync(FeatureFlag flag)
    {
        var existing = await GetAsync(flag.Key);
        if (existing is null)
        {
            await _repository.AddAsync(flag);
        }
        else
        {
            await _repository.UpdateAsync(flag, x => x.Key == flag.Key);
        }
    }

    public async Task DeleteAsync(string key)
    {
        var list = await _repository.GetListAsync(x => x.Key == key);
        var flag = list.FirstOrDefault();
        if (flag is not null)
        {
            await _repository.DeleteAsync(flag);
        }
    }
}
