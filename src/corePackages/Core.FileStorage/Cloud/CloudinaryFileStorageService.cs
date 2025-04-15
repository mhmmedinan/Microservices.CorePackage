using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;

namespace Core.FileStorage.Cloud;


public class CloudinaryFileStorageService : IFileStorageService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryFileStorageService(IOptions<CloudinaryFileStorageOptions> options)
    {
        var opts = options.Value;
        _cloudinary = new Cloudinary(new Account(opts.CloudName, opts.ApiKey, opts.ApiSecret));
    }

    public async Task<FileUploadResult> UploadAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        var uploadParams = new RawUploadParams
        {
            File = new FileDescription(fileName, fileStream),
            PublicId = Path.GetFileNameWithoutExtension(fileName)
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);

        return new FileUploadResult
        {
            Success = uploadResult.StatusCode == System.Net.HttpStatusCode.OK,
            FileUrl = uploadResult.Url.ToString(),
            PublicId = uploadResult.PublicId
        };
    }

    public async Task DeleteAsync(string fileUrl, CancellationToken cancellationToken = default)
    {
        var publicId = Path.GetFileNameWithoutExtension(fileUrl);
        await _cloudinary.DestroyAsync(new DeletionParams(publicId));
    }
}

