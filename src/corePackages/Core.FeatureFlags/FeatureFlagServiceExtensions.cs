using Core.FeatureFlags.Domain;
using Core.FeatureFlags.Repositories;
using Core.FeatureFlags.Services;
using Core.Persistence.Repositories.MongoDb;
using Core.Persistence.Repositories.MongoDb.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.FeatureFlags;

public static class FeatureFlagServiceExtensions
{
    public static IServiceCollection AddFeatureFlagServices(IServiceCollection services,IConfiguration configuration)
    {
        services
            .Configure<MongoConnectionSettings>(configuration.GetSection("FeatureFlagMongoDb"))
            .AddScoped<IMongoAsyncRepository<FeatureFlag>, MongoRepositoryBase<FeatureFlag>>()
            .AddScoped<IFeatureFlagRepository, MongoFeatureFlagRepository>()
            .AddScoped<IFeatureFlagService, FeatureFlagManager>();
        return services;
    }
}
