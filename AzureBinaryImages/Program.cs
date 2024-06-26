using Azure.Storage.Blobs;
using AzureBinaryImages.Services;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton(x => new BlobServiceClient(builder.Configuration.GetValue<string>("Azure:BlobStorage:ConnectionString")));
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<ComputerVisionClient>(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    var endpoint = config["Azure:CognitiveServices:Endpoint"];
    var key = config["Azure:CognitiveServices:Key"];
    return new ComputerVisionClient(new ApiKeyServiceClientCredentials(key))
    {
        Endpoint = endpoint
    };
});

// Register the ImageAnalysisService
builder.Services.AddSingleton<ImageAnalysisService>();
var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
