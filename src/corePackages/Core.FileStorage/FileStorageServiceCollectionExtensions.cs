using Core.FileStorage.Cloud;
using Core.FileStorage.Local;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.FileStorage;

public static class FileStorageServiceCollectionExtensions
{
    public static IServiceCollection AddFileStorageServices(this IServiceCollection services,IConfiguration configuration)
    {
        services.Configure<FileStorageOptions>(configuration.GetSection("FileStorage"));

        var provider = configuration.GetValue<string>("FileStorage:Provider")?.ToLower();

        return provider switch
        {
            "cloudinary" => services.AddScoped<IFileStorageService, CloudinaryFileStorageService>(),
            "local" => services.AddScoped<IFileStorageService, LocalFileStorageService>(),
            _ => throw new Exception("Unsupported file storage provider.")
        };
        
    }
}
