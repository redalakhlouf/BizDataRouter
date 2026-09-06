using BizDataRouter.Settings;
using BizDataRouter.Status;
using Minio;
using Minio.DataModel.Args;

namespace BizDataRouter.MinioStorage;

public sealed class MinioStorage
{
    private readonly MinioSettings _settings;
    private readonly IMinioClient _minioClient;
    private readonly PipelineStatus _status;

    public MinioStorage(MinioSettings settings, IMinioClient minioClient, PipelineStatus status)
    {
        _settings = settings;
        _minioClient = minioClient;
        _status = status;
    }

    public async Task CheckConnectionAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _minioClient.BucketExistsAsync(
                new BucketExistsArgs().WithBucket(_settings.BucketName),
                cancellationToken);

            _status.MinioConnected = true;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch
        {
            _status.MinioConnected = false;
        }
    }

    public async Task UploadAsync(string localFilePath, CancellationToken cancellationToken)
    {
        try
        {
            if (!File.Exists(localFilePath))
                throw new FileNotFoundException("Le fichier à envoyer est introuvable.", localFilePath);

            bool bucketExists = await _minioClient.BucketExistsAsync(
                new BucketExistsArgs().WithBucket(_settings.BucketName),
                cancellationToken);

            if (!bucketExists)
            {
                await _minioClient.MakeBucketAsync(
                    new MakeBucketArgs().WithBucket(_settings.BucketName),
                    cancellationToken);
            }

            string objectName = Path.GetFileName(localFilePath);
            await _minioClient.PutObjectAsync(
                new PutObjectArgs()
                    .WithBucket(_settings.BucketName)
                    .WithObject(objectName)
                    .WithFileName(localFilePath)
                    .WithContentType("text/csv"),
                cancellationToken);

            _status.MinioConnected = true;
            _status.LastUploadedFile = objectName;
            _status.LastUploadAt = DateTime.UtcNow;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch
        {
            _status.MinioConnected = false;
            throw;
        }
    }
}
