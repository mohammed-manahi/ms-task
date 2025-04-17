namespace AppService.Models;

public class ImageUploadResponse
{
    public string FileId { get; set; } = string.Empty;
    public string UploadUrl { get; set; } = string.Empty;
    public string VerificationCode { get; set; } = string.Empty;
}