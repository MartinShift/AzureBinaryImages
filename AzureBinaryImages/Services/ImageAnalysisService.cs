using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

namespace AzureBinaryImages.Services;
public class ImageAnalysisService
{
    private ComputerVisionClient _computerVisionClient;
    private readonly IConfiguration _configuration;

    public ImageAnalysisService(IConfiguration configuration)
    {
        _configuration = configuration;
        _computerVisionClient = Authenticate(_configuration.GetValue<string>("Azure:CognitiveServices:Key"), _configuration.GetValue<string>("Azure:CognitiveServices:Endpoint"));
    }

    public async Task<ImageAnalysis> AnalyzeImageAsync(string imageUrl)
    {
        _computerVisionClient = Authenticate(_configuration.GetValue<string>("Azure:CognitiveServices:Key"), _configuration.GetValue<string>("Azure:CognitiveServices:Endpoint"));
        var features = new List<VisualFeatureTypes?> { VisualFeatureTypes.Description, VisualFeatureTypes.Adult, VisualFeatureTypes.Faces, VisualFeatureTypes.Categories, VisualFeatureTypes.Tags, VisualFeatureTypes.Objects };
        var analysisResult = await _computerVisionClient.AnalyzeImageAsync(imageUrl, features);
        return analysisResult;
    }

    public static ComputerVisionClient Authenticate(string key, string endpoint)
    {
        ComputerVisionClient visionClient = new ComputerVisionClient(
        new ApiKeyServiceClientCredentials(key))
        { Endpoint = endpoint };
        return visionClient;
    }
}
