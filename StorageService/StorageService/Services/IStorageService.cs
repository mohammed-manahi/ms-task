using StorageService.Models;

namespace StorageService.Services;

public interface IStorageService
{
    Task<(string FileId, string UploadUrl, string VerificationCode)> GenerateUploadUrl(string fileName);
    Task<bool> SaveUploadedFile(string fileId, string verificationCode, IFormFile file);
}