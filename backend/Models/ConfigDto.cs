using BizDataRouter.Settings;

namespace BizDataRouter.Models;

/// <summary>Configuration affichable : les clés d'accès ne quittent jamais le serveur.</summary>
public sealed record ConfigDto(
    string PiApiUrl,
    string MinioEndpoint,
    string MinioBucketName,
    bool MinioUseSsl,
    long MaximumFileSizeMb,
    bool AccessKeyConfigured,
    bool SecretKeyConfigured)
{
    public static ConfigDto FromSettings(AppSettings settings) => new(
        settings.PiApi.Url,
        settings.Minio.Endpoint,
        settings.Minio.BucketName,
        settings.Minio.UseSsl,
        settings.Csv.MaximumFileSizeMb,
        !string.IsNullOrWhiteSpace(settings.Minio.AccessKey),
        !string.IsNullOrWhiteSpace(settings.Minio.SecretKey));
}
