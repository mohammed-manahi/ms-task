using Microsoft.AspNetCore.Mvc;
using StorageService.Models;
using StorageService.Services;

namespace StorageService.Controllers;

[ApiController]
[Route("api/storage")]
public class StorageController : ControllerBase
{
    private readonly IStorageService _storageService;
    private readonly IConfiguration _configuration;

    public StorageController(IStorageService storageService, IConfiguration configuration)
    {
        _storageService = storageService;
        _configuration = configuration;
    }

    [HttpPost("presigned-url")]
    public async Task<IActionResult> GetPresignedUrl([FromBody] PresignedUrlRequest request,
        [FromHeader(Name = "ApiKey")] string apiKey)
    {
        // Returns id, url and verification code from the storage service
        if (apiKey != _configuration.GetValue<string>("ApiKey"))
        {
            return Unauthorized();
        }

        var (fileId, uploadUrl, verificationCode) = await _storageService.GenerateUploadUrl(request.FileName);

        return Ok(new
        {
            fileId,
            uploadUrl,
            verificationCode
        });
    }

    [HttpPost("upload/{fileId}")]
    public async Task<IActionResult> UploadFile(string fileId, [FromQuery] string verificationCode, IFormFile file)
    {
        // Uploads the image based on the id and verification code
        if (file == null)
        {
            return BadRequest("No file uploaded");
        }

        var result = await _storageService.SaveUploadedFile(fileId, verificationCode, file);
        if (!result)
        {
            return BadRequest("Invalid file ID or verification code");
        }

        return Ok(new { fileId });
    }
}