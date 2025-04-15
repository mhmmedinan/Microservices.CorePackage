using Microsoft.Extensions.Options;
using Microsoft.Extensions.Hosting;

namespace Core.FileStorage.Local;

public class LocalFileStorageService : IFileStorageService
{
    private readonly IHostEnvironment _env;
    private readonly FileStorageOptions _options;

    public LocalFileStorageService(IHostEnvironment env, IOptions<FileStorageOptions> options)
    {
        _env = env;
        _options = options.Value;
    }

    public async Task<FileUploadResult> UploadAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        var path = Path.Combine(_env.ContentRootPath, _options.BasePath ?? "uploads");
        Directory.CreateDirectory(path);

        var filePath = Path.Combine(path, fileName);

        using var stream = new FileStream(filePath, FileMode.Create);
        await fileStream.CopyToAsync(stream, cancellationToken);

        return new FileUploadResult
        {
            Success = true,
            FileUrl = $"/{_options.BasePath}/{fileName}"
        };
    }

    public Task DeleteAsync(string fileUrl, CancellationToken cancellationToken = default)
    {
        var path = Path.Combine(_env.ContentRootPath, fileUrl.TrimStart('/'));
        if (File.Exists(path))
            File.Delete(path);
        return Task.CompletedTask;
    }
}
