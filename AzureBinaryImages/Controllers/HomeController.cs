using Azure.Storage.Blobs;
using AzureBinaryImages.Services;
using Microsoft.AspNetCore.Mvc;

public class HomeController(BlobServiceClient blobServiceClient, ImageAnalysisService imageAnalysisService, IConfiguration configuration) : Controller
{
    private readonly BlobServiceClient _blobServiceClient = blobServiceClient;
    private readonly ImageAnalysisService _imageAnalysisService = imageAnalysisService;
    private readonly string _containerName = configuration.GetValue<string>("Azure:BlobStorage:ContainerName");

    [HttpPost("upload")]
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        var blobClient = containerClient.GetBlobClient(file.FileName);
        await blobClient.UploadAsync(file.OpenReadStream(), true);

        // Analyze the uploaded image
        var imageUrl = blobClient.Uri.ToString();
        var analysisResult = await _imageAnalysisService.AnalyzeImageAsync(imageUrl);

        // Return the analysis result
        return Ok(new
        {
            ImageUrl = imageUrl,
            Description = analysisResult.Description.Captions[0]?.Text,
            IsAdultContent = analysisResult.Adult.IsAdultContent,
            AdultScore = analysisResult.Adult.AdultScore,
            IsRacyContent = analysisResult.Adult.IsRacyContent,
            RacyScore = analysisResult.Adult.RacyScore
        });
    }

    [HttpGet("/")]
    public IActionResult Index()
    {
        return View();
    }
}