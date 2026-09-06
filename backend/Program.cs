using BizDataRouter.MinioStorage;
using BizDataRouter.Services;
using BizDataRouter.Settings;
using BizDataRouter.Status;
using Minio;

var builder = WebApplication.CreateBuilder(args);

// La configuration est lue une fois au démarrage et partagée par les services.
var settings = builder.Configuration.Get<AppSettings>()
    ?? throw new InvalidOperationException("La configuration appsettings.json est invalide.");

builder.Services.AddSingleton(settings);
builder.Services.AddSingleton(settings.Minio);
builder.Services.AddSingleton<PipelineStatus>();

builder.Services.AddHttpClient<PiApiReader>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(10);
});

builder.Services.AddSingleton<IMinioClient>(_ =>
    new MinioClient()
        .WithEndpoint(settings.Minio.Endpoint)
        .WithCredentials(settings.Minio.AccessKey, settings.Minio.SecretKey)
        .WithSSL(settings.Minio.UseSsl)
        .Build());

builder.Services.AddSingleton<MinioStorage>();
builder.Services.AddSingleton<PipelineService>();
builder.Services.AddSingleton<PipelineCoordinator>();
builder.Services.AddHostedService(service => service.GetRequiredService<PipelineCoordinator>());

builder.Services.AddScoped<DataQueryService>();
builder.Services.AddScoped<MinioQueryService>();

builder.Services.AddControllers();

var app = builder.Build();

app.MapGet("/api/health", () => Results.Ok(new
{
    status = "healthy",
    timestamp = DateTime.UtcNow
}));

app.MapControllers();

app.Run();
