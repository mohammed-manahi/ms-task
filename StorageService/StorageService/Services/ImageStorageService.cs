using StorageService.Models;

namespace StorageService.Services;

public class ImageStorageService : IStorageService
{
       private readonly Dictionary<string, ImageMetadata> _filesStore = new();
        private readonly string _uploadPath;
        
        public ImageStorageService()
        {
            _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
            if (!Directory.Exists(_uploadPath))
            {
                Directory.CreateDirectory(_uploadPath);
            }
        }
        
        public Task<(string FileId, string UploadUrl, string VerificationCode)> GenerateUploadUrl(string fileName)
        {
            // Generates image id, url and verification code
            var imageMetadata = new ImageMetadata()
            {
                FileName = fileName
            };
            _filesStore[imageMetadata.Id] = imageMetadata;
            var uploadUrl = $"/api/storage/upload/{imageMetadata.Id}";
            return Task.FromResult((imageMetadata.Id, uploadUrl, imageMetadata.VerificationCode));
        }
        
        public async Task<bool> SaveUploadedFile(string fileId, string verificationCode, IFormFile file)
        {
            // Uploads image to local file system
            if (!_filesStore.TryGetValue(fileId, out var metadata))
            {
                return false;
            }
            if (metadata.VerificationCode != verificationCode)
            {
                return false;
            }
            var filePath = Path.Combine(_uploadPath, fileId);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            metadata.IsUploaded = true;
            _filesStore[fileId] = metadata;
            
            return true;
        }
        
}