using Core.CrossCuttingConcerns.Utilities.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Core.CrossCuttingConcerns.Utilities.Helpers;

public class FileHelper
{
    public static string AddAsync(IFormFile file, string basePath)
    {
        try
        {
            var result = newPath(file, basePath);
            var sourcePath = Path.GetTempFileName();
            using (var stream = new FileStream(sourcePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            File.Move(sourcePath, result.newPath);
            return result.Path2;

        }
        catch (Exception e)
        {
            return e.Message;
        }

    }


    public static string UpdateAsync(string sourcePath, IFormFile file, string basePath)
    {
        var result = newPath(file, basePath);
        try
        {
            if (sourcePath.Length > 0)
            {
                using (var stream = new FileStream(result.newPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
            }

            DeleteAsync(sourcePath);
        }
        catch (Exception e)
        {
            return e.Message;
        }
        return result.Path2;
    }


    public static DataResult<string> DeleteAsync(string path)
    {
        try
        {
            File.Delete(path);
            return DataResult<string>.IsSuccess("Successfully");
        }
        catch (Exception exception)
        {
            return DataResult<string>.IsError($"Error: {exception.Message}");
        }
    }

    private static (string newPath, string Path2) newPath(IFormFile file, string basePath)
    {
        string fileExtension = Path.GetExtension(file.FileName);
        var creatingFileName = Guid.NewGuid().ToString("N") + fileExtension;
        var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), $@"wwwroot\Images\" + basePath + $@"\");
        if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);
        var result = directoryPath + creatingFileName;
        return (result, $@"\Images\{basePath}\{creatingFileName}");

    }

}
