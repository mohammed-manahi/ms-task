using AppService.Models;
using AppService.Services;
using Microsoft.AspNetCore.Mvc;

namespace AppService.Controllers;

[ApiController]
[Route("api/upload")]
public class UploadController : ControllerBase
{
    private readonly IStorageClient _storageClient;
    private readonly IConfiguration _configuration;

    public UploadController(IStorageClient storageClient, IConfiguration configuration)
    {
        _storageClient = storageClient;
        _configuration = configuration;
    }

    [HttpPost("get-url")]
    public async Task<IActionResult> GetUploadUrl([FromBody] ImageUploadRequest request)
    {
        // Uploads the image url to the storage service
        try
        {
            var response = await _storageClient.GetUploadUrl(request.FileName);
            var baseUrl = $"{Request.Scheme}://{Request.Host.Value}";

            if (!response.UploadUrl.StartsWith("http"))
            {
                var storageServiceUrl = _configuration["StorageServiceUrl"] ?? "http://localhost:5201/";
                response.UploadUrl =
                    $"{storageServiceUrl}{response.UploadUrl}?verificationCode={response.VerificationCode}";
            }

            return Ok(new
            {
                fileId = response.FileId,
                uploadUrl = response.UploadUrl
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error: {ex.Message}");
        }
    }
}