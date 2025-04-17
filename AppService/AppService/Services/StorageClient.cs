using System.Text;
using System.Text.Json;
using AppService.Models;

namespace AppService.Services;

public class StorageClient : IStorageClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public StorageClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _httpClient.BaseAddress = new Uri(configuration["StorageServiceUrl"] ?? "http://localhost:5201/");
    }

    public async Task<ImageUploadResponse> GetUploadUrl(string fileName)
    {
        // Deserializes the image upload response
        var request = new
        {
            FileName = fileName
        };

        var content = new StringContent(
            JsonSerializer.Serialize(request),
            Encoding.UTF8,
            "application/json");

        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("ApiKey", _configuration.GetValue<string>("ApiKey"));

        var response = await _httpClient.PostAsync("api/storage/presigned-url", content);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<ImageUploadResponse>(responseContent,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

    public async Task<bool> ValidateImageId(string imageId)
    {
        // Validates the upload and returns boolean of validation result
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("ApiKey", _configuration.GetValue<string>("ApiKey"));

        var response = await _httpClient.GetAsync($"api/storage/validate/{imageId}");
        if (!response.IsSuccessStatusCode)
        {
            return false;
        }

        var content = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(content);
        return doc.RootElement.GetProperty("isValid").GetBoolean();
    }
}