namespace StorageService.Models;

public class ImageMetadata
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string FileName { get; set; } = string.Empty;
    public string VerificationCode { get; set; } = GenerateRandomCode();
    public bool IsUploaded { get; set; } = false;

    private static string GenerateRandomCode()
    {
        // Generate verification code of 8 characters
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, 8)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}