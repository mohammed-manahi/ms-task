using AppService.Models;

namespace AppService.Services;

public interface IStorageClient
{
    Task<ImageUploadResponse> GetUploadUrl(string fileName);
    Task<bool> ValidateImageId(string imageId);
}